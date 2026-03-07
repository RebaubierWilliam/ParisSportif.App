/**
 * extracteur_mesParis.js — Extracteur adaptatif ParionsSport
 * Mode 1 : page "Mes paris"  → psel-history-card
 * Mode 2 : page de cotes     → psel-event / psel-live-event
 */
(function () {
    'use strict';

    function log(msg) {
        if (window.chrome && window.chrome.webview)
            window.chrome.webview.postMessage(JSON.stringify({ type: 'LOG', message: msg }));
    }

    function sendParis(paris) {
        if (window.chrome && window.chrome.webview)
            window.chrome.webview.postMessage(JSON.stringify({ type: 'PARIS_EXTRACTED', data: paris }));
    }


    function matchSportStr(str) {
        const s = str.toLowerCase();
        if (s.includes('football') || s.includes('foot') || s.includes('soccer')) return 'Football';
        if (s.includes('tennis'))                                                  return 'Tennis';
        if (s.includes('basketball') || s.includes('basket'))                     return 'Basketball';
        if (s.includes('handball'))                                                return 'Handball';
        if (s.includes('volleyball') || s.includes('volley'))                     return 'Volley Ball';
        if (s.includes('rugby'))                                                   return 'Rugby';
        if (s.includes('hockey'))                                                  return 'Hockey';
        if (s.includes('snooker'))                                                 return 'Snooker';
        return null;
    }

    function detecterSport(root) {
        // 1. Essayer plusieurs sélecteurs d'icône de sport
        const iconSelectors = [
            '[class*="sprite-icon-"]',
            '[class*="sport-icon"]',
            '[class*="icon-sport"]',
            '[class*="discipline"]',
            '[class*="sport-"]',
        ];
        for (const sel of iconSelectors) {
            const el = root.querySelector(sel);
            if (!el) continue;
            const combined = [el.className, el.getAttribute('aria-label') || '', el.getAttribute('alt') || ''].join(' ');
            const r = matchSportStr(combined);
            if (r) return r;
        }

        // 2. Chercher dans les ancêtres (jusqu'à 4 niveaux)
        let ancestor = root.parentElement;
        for (let i = 0; i < 4 && ancestor; i++, ancestor = ancestor.parentElement) {
            const r = matchSportStr(ancestor.className || '');
            if (r) return r;
        }

        // 3. Fallback sur l'URL
        const r = matchSportStr(window.location.pathname);
        if (r) return r;

        log('⚠️ Sport non détecté (Autre) pour : ' + root.className.slice(0, 80));
        return 'Autre';
    }

    // ══════════════════════════════════════════════════════════════════
    //  MODE 1 — Mes paris (psel-history-card)
    // ══════════════════════════════════════════════════════════════════
    function extracterMesParis() {
        const cards = document.querySelectorAll('psel-history-card');
        if (cards.length === 0) return null;

        const paris = [];
        cards.forEach(card => {
            try {
                const p = {};

                p.type = card.querySelector('.psel-history-card__title')?.textContent.trim() ?? 'Simple';

                p.sport = detecterSport(card);

                p.equipes = [];
                card.querySelectorAll('.psel-opponent__name').forEach((o, i) => {
                    if (i < 2) p.equipes.push(o.textContent.trim());
                });

                p.score        = card.querySelector('.psel-score--current')?.textContent.replace(/\s+/g, ' ').trim() ?? null;
                const liveEl   = card.querySelector('psel-live-timer');
                const mktLabel = card.querySelector('.psel-cart-market__label');
                const mktText  = mktLabel?.textContent.replace(/\s+/g, ' ').trim() ?? '';
                p.isLive       = !!(liveEl || mktText.includes('Live'));
                p.minuteOuHeure= card.querySelector('.psel-timer')?.textContent.replace(/\s+/g, ' ').trim() ?? 'N/A';
                p.marche       = mktText;
                p.selection    = mktLabel?.querySelector('.psel-font-bold')?.textContent.trim() ?? 'N/A';
                // Cote individuelle — plusieurs sélecteurs en cascade
                const coteSelectors = [
                    '.psel-cart-market__value',
                    '[class*="market__value"]',
                    '[class*="cart-market__value"]',
                    '[class*="odd__value"]',
                    '[class*="odd-value"]',
                    '[class*="psel-odd"]',
                    '[class*="price"]',
                ];
                let coteVal = 'N/A';
                for (const sel of coteSelectors) {
                    const el = card.querySelector(sel);
                    if (el) { const t = el.textContent.trim(); if (t) { coteVal = t; break; } }
                }
                p.cote = coteVal;

                p.coteTotale = 'N/A'; p.mise = 'N/A'; p.gainsPotentiels = 'N/A';
                card.querySelectorAll('.psel-history-card__summary__item').forEach(item => {
                    const lbl = item.querySelector('.psel-text-sm')?.textContent.trim() ?? '';
                    const val = item.querySelector('.psel-history-card__summary__item__amount')?.textContent.trim() ?? '';
                    if (lbl.includes('Cote totale'))                            p.coteTotale     = val;
                    if (lbl.includes('Cote') && !lbl.includes('totale') && val) p.cote           = val;
                    if (lbl.includes('Mise'))                                    p.mise            = val;
                    if (lbl.includes('Gains'))                                   p.gainsPotentiels = val;
                });

                // Fallback regex dans mktText
                if (!p.cote || p.cote === 'N/A') {
                    const m = mktText.match(/\b(\d{1,2}[.,]\d{2})\b/);
                    if (m) p.cote = m[1];
                }

                const cashoutBtn = card.querySelector('.psel-button--cashout');
                if (cashoutBtn) {
                    const txt = cashoutBtn.textContent.replace(/\s+/g, ' ').trim();
                    const m   = txt.match(/Cash\s*Out\s*[:\s]+([\d,]+\s*€)/i);
                    p.cashout = m ? m[1].trim() : txt.replace(/Cash\s*Out\s*:/i, '').trim();
                } else {
                    p.cashout = null;
                }

                p.numeroPari = 'N/A'; p.datePari = 'N/A';
                card.querySelector('.psel-history-card__info')
                    ?.querySelectorAll('span').forEach(span => {
                        const t = span.textContent.trim();
                        if (t.startsWith('Pari')) p.numeroPari = t;
                        if (t.startsWith('Le '))  p.datePari   = t;
                    });

                // Détection pari Question (Oui/Non, buteur, essai…)
                const selLower = (p.selection || '').toLowerCase().trim();
                const mkLower  = (p.marche || '').toLowerCase();
                if (selLower === 'oui' || selLower === 'non' || selLower === 'yes' || selLower === 'no'
                    || mkLower.includes('?') || mkLower.includes('buteur')
                    || mkLower.includes('marque') || mkLower.includes('essai')
                    || mkLower.includes('scorer') || mkLower.includes('réalise')) {
                    p.type = 'Question';
                }

                paris.push(p);
            } catch (err) { log('Erreur card: ' + err.message); }
        });

        return paris;
    }

    // ══════════════════════════════════════════════════════════════════
    //  MODE 2 — Page de cotes (événements disponibles)
    // ══════════════════════════════════════════════════════════════════
    function extracterPageCotes() {
        // Sélecteur réel confirmé par dump HTML : psel-event-main
        const EVENT_SELECTORS = [
            'psel-event-main',
            'psel-event-card', 'psel-live-event-card',
            '.psel-event', '[class*="event-card"]',
        ];

        function sportDepuisHref(href) {
            if (!href) return 'Autre';
            return matchSportStr(href) ?? 'Autre';
        }

        let containers = [];
        for (const s of EVENT_SELECTORS) {
            const found = document.querySelectorAll(s);
            if (found.length > 0) { containers = [...found]; break; }
        }
        if (containers.length === 0) return null;

        const paris = [];
        let idx = 0;
        containers.forEach(el => {
            try {
                // Équipes
                const equipes = Array.from(el.querySelectorAll('.psel-opponent__name'))
                    .slice(0, 2).map(t => t.textContent.trim());
                if (equipes.length === 0) return;

                // Heure / statut
                const minuteOuHeure = el.querySelector('time.psel-timer, .psel-timer')
                    ?.textContent.trim() ?? 'N/A';

                // Compétition (marché)
                const marche = el.querySelector('.psel-event-info__competition')
                    ?.textContent.trim() ?? '';

                // Score (si live)
                const score = el.querySelector('.psel-scoreboard__score, .psel-score--current, [class*="score--"]')
                    ?.textContent.replace(/\s+/g, ' ').trim() ?? null;

                // Live
                const isLive = !!el.querySelector('psel-live-timer, [class*="live-timer"], [class*="liveTimer"]');

                // Sport depuis l'href de l'événement
                const href  = el.querySelector('a.psel-event__link, a[href*="/paris-"]')?.getAttribute('href') ?? '';
                const sport = sportDepuisHref(href) || detecterSport(el);

                // Cotes + popularité : une ligne par match avec toutes les cotes groupées
                const outcomes = Array.from(el.querySelectorAll('psel-outcome'));
                const labels = outcomes
                    .map(o => o.querySelector('.psel-outcome__label')?.textContent.trim() ?? '')
                    .filter(Boolean);
                const cotes = outcomes
                    .map(o => o.querySelector('.psel-outcome__data')?.textContent.trim() ?? '')
                    .filter(Boolean);

                // Pourcentages de paris (% parieurs par issue) — plusieurs sélecteurs pour robustesse
                const PCT_SELECTORS = [
                    '.psel-outcome__popularity',
                    '.psel-outcome__percentage',
                    '[class*="outcome__popularity"]',
                    '[class*="outcome__percentage"]',
                    '[class*="outcome__percent"]',
                    '[class*="popularity__value"]',
                    '[class*="popularity-value"]',
                    '[class*="percent-value"]',
                    '[class*="bets-percent"]',
                    '[class*="betPercent"]',
                    '[class*="bet-percent"]',
                ];
                const pourcentages = outcomes.map(o => {
                    for (const sel of PCT_SELECTORS) {
                        const el2 = o.querySelector(sel);
                        if (el2) {
                            const t = el2.textContent.trim();
                            if (t) return t;
                        }
                    }
                    // Fallback : chercher un texte qui ressemble à un pourcentage dans l'outcome
                    const all = o.querySelectorAll('*');
                    for (const node of all) {
                        const t = node.textContent.trim();
                        if (/^\d{1,3}\s*%$/.test(t)) return t;
                    }
                    return '';
                }).filter(Boolean);

                // Détection pari Question (Oui/Non, buteur, essai…)
                const selText = labels.join(' / ').toLowerCase();
                const mkLower2 = (marche || '').toLowerCase();
                const isQuestion = selText === 'oui / non' || selText === 'oui' || selText === 'non'
                    || mkLower2.includes('?') || mkLower2.includes('buteur')
                    || mkLower2.includes('marque') || mkLower2.includes('essai')
                    || mkLower2.includes('scorer') || mkLower2.includes('réalise');

                paris.push({
                    type: isQuestion ? 'Question' : 'Disponible', sport, equipes, score, isLive,
                    minuteOuHeure, marche,
                    selection:         labels.join(' / ')        || 'N/A',
                    cote:              cotes.join(' / ')         || 'N/A',
                    pourcentagesParis: pourcentages,
                    coteTotale: 'N/A', mise: '', gainsPotentiels: '',
                    cashout: null, numeroPari: `evt-${++idx}`, datePari: 'N/A',
                });
            } catch (err) { log('Erreur event: ' + err.message); }
        });

        return paris.length > 0 ? paris : null;
    }

    // ══════════════════════════════════════════════════════════════════
    //  MODE 3 — Page détail match : marchés question (buteur, essai…)
    // ══════════════════════════════════════════════════════════════════
    function extracterMarchesQuestion() {
        // Sélecteurs pour les blocs de marché dans une page de détail de match
        const MARKET_SELECTORS = [
            'psel-market',
            'psel-market-group',
            '[class*="market-group"]',
            '[class*="market-container"]',
            '.psel-markets__item',
            '[class*="markets__item"]',
        ];

        let markets = [];
        for (const s of MARKET_SELECTORS) {
            const found = document.querySelectorAll(s);
            if (found.length > 0) { markets = [...found]; break; }
        }

        // Fallback : chercher n'importe quel conteneur qui a des psel-outcome dedans
        if (markets.length === 0) {
            const allOutcomes = document.querySelectorAll('psel-outcome');
            if (allOutcomes.length === 0) return null;

            // Remonter aux parents contenant des groupes d'outcomes
            const parentSet = new Set();
            allOutcomes.forEach(o => {
                let p = o.parentElement;
                // Remonter de 1 à 3 niveaux pour trouver le conteneur de marché
                for (let i = 0; i < 3 && p; i++) {
                    if (p.querySelectorAll('psel-outcome').length >= 2) {
                        parentSet.add(p);
                        break;
                    }
                    p = p.parentElement;
                }
            });
            markets = [...parentSet];
        }

        if (markets.length === 0) return null;

        // Récupérer le contexte global du match (équipes, sport, heure)
        const TITLE_SELECTORS = [
            '.psel-event-header__title',
            '.psel-breadcrumb',
            'h1', '[class*="event-header"]',
            '[class*="match-header"]',
            '.psel-opponent__name',
        ];
        let equipes = [];
        const opps = document.querySelectorAll('.psel-opponent__name');
        if (opps.length >= 2) {
            equipes = [opps[0].textContent.trim(), opps[1].textContent.trim()];
        } else {
            // Essayer depuis le titre de la page ou le breadcrumb
            for (const sel of TITLE_SELECTORS) {
                const el = document.querySelector(sel);
                if (el) {
                    const txt = el.textContent.trim();
                    const m = txt.match(/(.+?)\s*[-–vs.]+\s*(.+)/);
                    if (m) { equipes = [m[1].trim(), m[2].trim()]; break; }
                }
            }
        }
        if (equipes.length === 0) equipes = ['Équipe A', 'Équipe B'];

        const sport = matchSportStr(window.location.pathname) || detecterSport(document.body) || 'Autre';
        const isLive = !!document.querySelector('psel-live-timer, [class*="live-timer"], [class*="liveTimer"]');
        const score = document.querySelector('.psel-scoreboard__score, .psel-score--current, [class*="score--"]')
            ?.textContent.replace(/\s+/g, ' ').trim() ?? null;
        const minuteOuHeure = document.querySelector('time.psel-timer, .psel-timer')
            ?.textContent.trim() ?? 'N/A';

        const paris = [];
        let idx = 0;

        markets.forEach(mkt => {
            try {
                // Titre du marché (= la question)
                const TITLE_MKT_SEL = [
                    '.psel-market__title',
                    '.psel-market-group__title',
                    '[class*="market__title"]',
                    '[class*="market-title"]',
                    'h2', 'h3', 'h4',
                    '.psel-text-bold',
                ];
                let marcheTitle = '';
                for (const sel of TITLE_MKT_SEL) {
                    const el = mkt.querySelector(sel);
                    if (el) {
                        const t = el.textContent.replace(/\s+/g, ' ').trim();
                        if (t.length > 2 && t.length < 200) { marcheTitle = t; break; }
                    }
                }
                if (!marcheTitle) return; // Pas de titre = pas un marché exploitable

                // Outcomes (labels + cotes)
                const outcomes = Array.from(mkt.querySelectorAll('psel-outcome'));
                if (outcomes.length === 0) return;

                const labels = outcomes
                    .map(o => {
                        const lbl = o.querySelector('.psel-outcome__label');
                        return lbl ? lbl.textContent.trim() : '';
                    })
                    .filter(Boolean);
                const cotes = outcomes
                    .map(o => {
                        const d = o.querySelector('.psel-outcome__data');
                        return d ? d.textContent.trim() : '';
                    })
                    .filter(Boolean);

                if (labels.length === 0 || cotes.length === 0) return;

                // Pourcentages (si disponibles)
                const PCT_SELECTORS = [
                    '.psel-outcome__popularity',
                    '.psel-outcome__percentage',
                    '[class*="outcome__popularity"]',
                    '[class*="outcome__percentage"]',
                ];
                const pourcentages = outcomes.map(o => {
                    for (const sel of PCT_SELECTORS) {
                        const el2 = o.querySelector(sel);
                        if (el2) { const t = el2.textContent.trim(); if (t) return t; }
                    }
                    const all = o.querySelectorAll('*');
                    for (const node of all) {
                        const t = node.textContent.trim();
                        if (/^\d{1,3}\s*%$/.test(t)) return t;
                    }
                    return '';
                }).filter(Boolean);

                paris.push({
                    type: 'Question',
                    sport,
                    equipes,
                    score,
                    isLive,
                    minuteOuHeure,
                    marche: marcheTitle,
                    selection: labels.join(' / ') || 'N/A',
                    cote: cotes.join(' / ') || 'N/A',
                    pourcentagesParis: pourcentages,
                    coteTotale: 'N/A',
                    mise: '',
                    gainsPotentiels: '',
                    cashout: null,
                    numeroPari: `q-${++idx}`,
                    datePari: 'N/A',
                });
            } catch (err) { log('Erreur marché question: ' + err.message); }
        });

        return paris.length > 0 ? paris : null;
    }

    // ══════════════════════════════════════════════════════════════════
    //  MODE 4 — Dump diagnostic HTML (pour debug)
    // ══════════════════════════════════════════════════════════════════
    function dumpPageStructure() {
        const tags = new Set();
        document.querySelectorAll('*').forEach(el => {
            const tag = el.tagName.toLowerCase();
            if (tag.startsWith('psel-')) tags.add(tag);
        });
        log('📋 Éléments psel-* trouvés sur cette page : ' + [...tags].sort().join(', '));

        // Compter les outcomes et markets
        const nbOutcomes = document.querySelectorAll('psel-outcome').length;
        const nbMarkets = document.querySelectorAll('psel-market, psel-market-group, [class*="market"]').length;
        log(`📊 psel-outcome: ${nbOutcomes} | markets: ${nbMarkets}`);
    }

    // ══════════════════════════════════════════════════════════════════
    //  Point d'entrée
    // ══════════════════════════════════════════════════════════════════
    log('🔍 Détection du type de page…');
    dumpPageStructure();

    const paris = extracterMesParis();
    if (paris !== null) {
        log(`✅ Mode "Mes paris" — ${paris.length} paris trouvés`);
        sendParis(paris);
        return;
    }

    const cotes = extracterPageCotes();
    const questions = extracterMarchesQuestion();

    // Combiner cotes classiques + marchés question si les deux existent
    if (cotes !== null || questions !== null) {
        const combined = [...(cotes || []), ...(questions || [])];
        const nbCotes = cotes ? cotes.length : 0;
        const nbQ = questions ? questions.length : 0;
        log(`✅ ${nbCotes} cotes classiques + ${nbQ} marchés question — ${combined.length} total`);
        sendParis(combined);
        return;
    }

    log('⚠️ Aucune donnée reconnue sur cette page. Es-tu bien sur une page ParionsSport avec des paris ou des cotes ?');

})();
