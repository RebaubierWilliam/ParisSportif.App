/**
 * extracteur_mesParis.js — Extracteur adaptatif ParionsSport
 * Mode 1 : page "Mes paris"  → psel-history-card (ancien site)
 * Mode 2 : page de cotes     → psel-event / psel-live-event (ancien site)
 * Mode 3 : marchés question  → psel-market (ancien site)
 * Mode 4 : page paris ouverts → div.match-home (nouveau site 2026)
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
        if (s.includes('tennis-de-table') || s.includes('tennis de table') ||
            s.includes('table-tennis')   || s.includes('table tennis')   ||
            s.includes('ping-pong')      || s.includes('ping pong'))             return 'Tennis de table';
        if (s.includes('tennis'))                                                  return 'Tennis';
        if (s.includes('basketball') || s.includes('basket'))                     return 'Basketball';
        if (s.includes('handball'))                                                return 'Handball';
        if (s.includes('volleyball') || s.includes('volley'))                     return 'Volley Ball';
        if (s.includes('rugby'))                                                   return 'Rugby';
        if (s.includes('hockey sur glace') || s.includes('ice hockey') || s.includes('hockey')) return 'Hockey';
        if (s.includes('snooker'))                                                 return 'Snooker';
        return null;
    }

    function detecterSport(root) {
        // 1. Nouveau site : data attribute "tp-pictoSport-XXX" ou xlink:href="#sport"
        const sportData = root.querySelector('[data*="tp-pictoSport-"]');
        if (sportData) {
            const dataAttr = sportData.getAttribute('data') || '';
            const m = dataAttr.match(/tp-pictoSport-(.+)/);
            if (m) {
                const r = matchSportStr(m[1]);
                if (r) return r;
            }
        }
        const useEl = root.querySelector('svg use[xlink\\:href]');
        if (useEl) {
            const href = useEl.getAttribute('xlink:href') || '';
            const r = matchSportStr(href.replace('#', ''));
            if (r) return r;
        }

        // 2. Ancien site : sélecteurs d'icône de sport
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

        // 3. Chercher dans les ancêtres (jusqu'à 4 niveaux)
        let ancestor = root.parentElement;
        for (let i = 0; i < 4 && ancestor; i++, ancestor = ancestor.parentElement) {
            const r = matchSportStr(ancestor.className || '');
            if (r) return r;
        }

        // 4. Fallback sur l'URL
        const r = matchSportStr(window.location.pathname);
        if (r) return r;

        log('⚠️ Sport non détecté (Autre) pour : ' + root.className.slice(0, 80));
        return 'Autre';
    }

    // ══════════════════════════════════════════════════════════════════
    //  MODE 1 — Mes paris (psel-history-card) — ancien site
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

                // Fallback pour paris sans équipes (multi-but de ligue, etc.)
                if (p.equipes.length === 0) {
                    const ALT_SEL = [
                        '.psel-event-info__title',
                        '.psel-event-info__competition',
                        '[class*="event-title"]',
                        '[class*="competition"]',
                        '[class*="event-name"]',
                    ];
                    for (const sel of ALT_SEL) {
                        const found = card.querySelector(sel);
                        if (found) {
                            const t = found.textContent.replace(/\s+/g, ' ').trim();
                            if (t.length > 2 && t.length < 150) { p.equipes = [t]; break; }
                        }
                    }
                }

                p.score        = card.querySelector('.psel-score--current')?.textContent.replace(/\s+/g, ' ').trim() ?? null;
                const liveEl   = card.querySelector('psel-live-timer');
                const mktLabel = card.querySelector('.psel-cart-market__label');
                const mktText  = mktLabel?.textContent.replace(/\s+/g, ' ').trim() ?? '';
                // psel-bet-option contient la vraie description pour les CB / questions
                const betOpt   = card.querySelector('.psel-bet-option')?.textContent.replace(/\s+/g, ' ').trim() ?? '';
                p.isLive       = !!(liveEl || mktText.includes('Live'));
                p.minuteOuHeure= card.querySelector('.psel-timer')?.textContent.replace(/\s+/g, ' ').trim() ?? 'N/A';
                p.marche       = betOpt || mktText;
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
                const typeLower = (p.type || '').toLowerCase();
                if (selLower === 'oui' || selLower === 'non' || selLower === 'yes' || selLower === 'no'
                    || mkLower.includes('?') || mkLower.includes('buteur')
                    || mkLower.includes('marque') || mkLower.includes('essai')
                    || mkLower.includes('scorer') || mkLower.includes('réalise')
                    || mkLower.includes('assist') || mkLower.includes('carton')
                    || mkLower.includes('tir cadré') || mkLower.includes('performance')) {
                    p.type = 'Question';
                }

                // Détection pari Multi But de Ligue
                if (typeLower.includes('multi but') || typeLower.includes('multi-but')
                    || mkLower.includes('multi but') || mkLower.includes('multi-but')
                    || mkLower.includes('multi goal') || mkLower.includes('nombre total de buts')) {
                    p.type = 'Multi But';
                }

                paris.push(p);
            } catch (err) { log('Erreur card: ' + err.message); }
        });

        return paris;
    }

    // ══════════════════════════════════════════════════════════════════
    //  MODE 2 — Page de cotes ancien site (psel-event)
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
                let equipes = Array.from(el.querySelectorAll('.psel-opponent__name'))
                    .slice(0, 2).map(t => t.textContent.trim());

                // Fallback pour les paris sans équipes (multi-but de ligue, etc.)
                if (equipes.length === 0) {
                    const ALT_NAME_SEL = [
                        '.psel-event-info__title',
                        '.psel-event-info__competition',
                        '[class*="event-info__title"]',
                        '[class*="event__title"]',
                        '[class*="event-name"]',
                        'h3', 'h4',
                    ];
                    for (const sel of ALT_NAME_SEL) {
                        const found = el.querySelector(sel);
                        if (found) {
                            const t = found.textContent.replace(/\s+/g, ' ').trim();
                            if (t.length > 2 && t.length < 150) { equipes = [t]; break; }
                        }
                    }
                    // Dernier recours : texte du lien principal
                    if (equipes.length === 0) {
                        const link = el.querySelector('a[href]');
                        if (link) {
                            const t = link.textContent.replace(/\s+/g, ' ').trim();
                            if (t.length > 2 && t.length < 150) equipes = [t];
                        }
                    }
                    if (equipes.length === 0) {
                        log('⚠️ Événement ignoré (aucun nom trouvé) : ' + el.tagName);
                        return;
                    }
                }

                // Heure / statut
                const minuteOuHeure = el.querySelector('time.psel-timer, .psel-timer')
                    ?.textContent.trim() ?? 'N/A';

                // Description du pari (psel-bet-option = clé pour les CB / questions / multi-but)
                const betOption = el.querySelector('.psel-bet-option')
                    ?.textContent.replace(/\s+/g, ' ').trim() ?? '';
                const competition = el.querySelector('.psel-event-info__competition')
                    ?.textContent.trim() ?? '';
                // Priorité au bet-option (description du pari), sinon la compétition
                const marche = betOption || competition;

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
                let labels = outcomes
                    .map(o => o.querySelector('.psel-outcome__label')?.textContent.trim() ?? '')
                    .filter(Boolean);
                const cotes = outcomes
                    .map(o => o.querySelector('.psel-outcome__data')?.textContent.trim() ?? '')
                    .filter(Boolean);

                // Fallback labels : layout psel-market--default (multi-but, vainqueur…)
                // Les labels sont dans <th class="psel-market__head"> au lieu de psel-outcome__label
                if (labels.length === 0) {
                    labels = Array.from(el.querySelectorAll('.psel-market__head'))
                        .map(th => th.textContent.trim())
                        .filter(Boolean);
                }

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
                const compLower = (competition || '').toLowerCase();
                const isQuestion = selText === 'oui / non' || selText === 'oui' || selText === 'non'
                    || mkLower2.includes('?') || mkLower2.includes('buteur')
                    || mkLower2.includes('marque') || mkLower2.includes('essai')
                    || mkLower2.includes('scorer') || mkLower2.includes('réalise')
                    || mkLower2.includes('assist') || mkLower2.includes('carton')
                    || mkLower2.includes('tir cadré') || mkLower2.includes('performance')
                    || mkLower2.includes('mi-temps') || mkLower2.startsWith('cb ');

                // Détection Multi But de Ligue
                const isMultiBut = mkLower2.includes('multi but') || mkLower2.includes('multi-but')
                    || mkLower2.includes('multi goal') || mkLower2.includes('nombre total de buts')
                    || compLower.includes('multibut') || compLower === 'multibuts'
                    || selText.includes('multi but') || selText.includes('multi-but')
                    || compLower.includes('multibuteur');

                const betType = isMultiBut ? 'Multi But' : isQuestion ? 'Question' : 'Disponible';

                paris.push({
                    type: betType, sport, equipes, score, isLive,
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
    //  MODE 3 — Page détail match ancien site : marchés question
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
    //  MODE 4 — Nouveau site 2026 : div.match-home (paris ouverts)
    //  Structure : div.match-home contenant :
    //    a.match-home_left[href] → lien + icône sport (svg use xlink:href="#tennis")
    //    div.match-home_title    → équipes (ex: "S.Korda-M.Landaluce")
    //    span[data*="ligueTitle"]→ ligue (ex: "Miami H")
    //    p.match-home_time       → marché + heure (ex: "N°416 1/N/2 I Fin de valid. 24/03 15h55")
    //    app-market-template     → cotes :
    //      button.outcomeButton  → .outcomeButton_type (1/N/2) + .outcomeButton_value (cote)
    //    span.outcome-repartition-percentage → pourcentages
    // ══════════════════════════════════════════════════════════════════
    function extracterNouveauSite() {
        const matchDivs = document.querySelectorAll('.match-home');
        if (matchDivs.length === 0) return null;

        function sportDepuisHref(href) {
            if (!href) return null;
            return matchSportStr(href);
        }

        const paris = [];
        let idx = 0;

        matchDivs.forEach(el => {
            try {
                // Équipes depuis .match-home_title (format "Équipe1-Équipe2")
                const titleEl = el.querySelector('.match-home_title');
                const titleText = titleEl?.textContent.replace(/\s+/g, ' ').trim() ?? '';
                if (!titleText) return;

                let equipes = [];
                // Séparer les équipes par "-" mais attention aux noms composés
                // Le format est "J.Lehecka-T.Fritz" ou "Miami F 2026" (pour les compétitions)
                const dashMatch = titleText.match(/^(.+?)\s*-\s*(.+)$/);
                if (dashMatch) {
                    equipes = [dashMatch[1].trim(), dashMatch[2].trim()];
                } else {
                    equipes = [titleText];
                }

                // Ligue / compétition
                const ligueEl = el.querySelector('[data*="ligueTitle"]');
                const ligue = ligueEl?.textContent.replace(/\s+/g, ' ').trim() ?? '';

                // Heure / marché depuis .match-home_time
                const timeEl = el.querySelector('.match-home_time');
                const timeText = timeEl?.textContent.replace(/\s+/g, ' ').trim() ?? 'N/A';

                // Extraire le type de marché et l'heure de validité
                // Format typique : "N°416 1/N/2 I Fin de valid. 24/03 15h55"
                let marche = ligue;
                let minuteOuHeure = 'N/A';
                if (timeText !== 'N/A') {
                    // Extraire la fin de validité
                    const validMatch = timeText.match(/Fin de valid\.\s*(.+)/i);
                    if (validMatch) {
                        minuteOuHeure = validMatch[1].trim();
                    }
                    // Le type de marché est dans le timeText (ex: "1/N/2", "Plus/Moins")
                    const mktMatch = timeText.match(/N°\d+\s+(.+?)\s+I\s/i);
                    if (mktMatch) {
                        marche = mktMatch[1].trim() + (ligue ? ' — ' + ligue : '');
                    } else {
                        marche = timeText + (ligue ? ' — ' + ligue : '');
                    }
                }

                // Sport : depuis href du lien, icône SVG, ou data attribute
                const linkEl = el.querySelector('a.match-home_left[href], a[href*="/paris-"]');
                const href = linkEl?.getAttribute('href') ?? '';
                let sport = sportDepuisHref(href) || detecterSport(el) || 'Autre';

                // Score (si live — chercher des éléments de score)
                const score = el.querySelector('[class*="score--"], [class*="scoreboard"]')
                    ?.textContent.replace(/\s+/g, ' ').trim() ?? null;

                // Live detection
                const isLive = !!el.querySelector('[class*="live-timer"], [class*="liveTimer"], [class*="live-icon"]')
                    || timeText.toLowerCase().includes('live')
                    || timeText.toLowerCase().includes('en cours');

                // Cotes depuis outcomeButton
                const outcomeButtons = Array.from(el.querySelectorAll('.outcomeButton'));
                const labels = outcomeButtons
                    .map(btn => btn.querySelector('.outcomeButton_type')?.textContent.trim() ?? '')
                    .filter(Boolean);
                const cotes = outcomeButtons
                    .map(btn => btn.querySelector('.outcomeButton_value')?.textContent.trim() ?? '')
                    .filter(Boolean);

                // Pourcentages de répartition
                const pourcentages = Array.from(el.querySelectorAll('.outcome-repartition-percentage'))
                    .map(pct => pct.textContent.trim())
                    .filter(Boolean);

                // Détection pari Question
                const selText = labels.join(' / ').toLowerCase();
                const mkLower = marche.toLowerCase();
                const isQuestion = selText === 'oui / non' || selText === 'oui' || selText === 'non'
                    || mkLower.includes('?') || mkLower.includes('buteur')
                    || mkLower.includes('marque') || mkLower.includes('essai')
                    || mkLower.includes('scorer') || mkLower.includes('réalise')
                    || mkLower.includes('assist') || mkLower.includes('carton')
                    || mkLower.includes('tir cadré') || mkLower.includes('performance')
                    || mkLower.includes('mi-temps') || mkLower.startsWith('cb ');

                // Détection Multi But de Ligue
                const isMultiBut = mkLower.includes('multi but') || mkLower.includes('multi-but')
                    || mkLower.includes('multi goal') || mkLower.includes('nombre total de buts');

                const betType = isMultiBut ? 'Multi But' : isQuestion ? 'Question' : 'Disponible';

                // Date depuis le date-competition parent
                let datePari = 'N/A';
                let dateSection = el.closest('.large-container');
                if (dateSection) {
                    const dateDiv = dateSection.previousElementSibling;
                    if (dateDiv && dateDiv.classList.contains('date-competition')) {
                        datePari = dateDiv.textContent.replace(/\s+/g, ' ').trim();
                    }
                }
                // Fallback : chercher le date-competition le plus proche en remontant
                if (datePari === 'N/A') {
                    let prev = el.closest('.large-container, .match-home-container');
                    while (prev) {
                        prev = prev.previousElementSibling;
                        if (prev && prev.classList.contains('date-competition')) {
                            datePari = prev.textContent.replace(/\s+/g, ' ').trim();
                            break;
                        }
                        if (!prev) break;
                    }
                }

                paris.push({
                    type: betType,
                    sport,
                    equipes,
                    score,
                    isLive,
                    minuteOuHeure,
                    marche,
                    selection:         labels.join(' / ')  || 'N/A',
                    cote:              cotes.join(' / ')   || 'N/A',
                    pourcentagesParis: pourcentages,
                    coteTotale: 'N/A',
                    mise: '',
                    gainsPotentiels: '',
                    cashout: null,
                    numeroPari: `nv-${++idx}`,
                    datePari,
                    ligue,
                    lienMatch: href,
                });
            } catch (err) { log('Erreur match-home: ' + err.message); }
        });

        return paris.length > 0 ? paris : null;
    }

    // ══════════════════════════════════════════════════════════════════
    //  Dump diagnostic HTML (pour debug)
    // ══════════════════════════════════════════════════════════════════
    function dumpPageStructure() {
        // Ancien site : éléments psel-*
        const pselTags = new Set();
        document.querySelectorAll('*').forEach(el => {
            const tag = el.tagName.toLowerCase();
            if (tag.startsWith('psel-')) pselTags.add(tag);
        });
        if (pselTags.size > 0) {
            log('📋 Éléments psel-* trouvés : ' + [...pselTags].sort().join(', '));
        }

        // Nouveau site : éléments match-home + app-*
        const appTags = new Set();
        document.querySelectorAll('*').forEach(el => {
            const tag = el.tagName.toLowerCase();
            if (tag.startsWith('app-')) appTags.add(tag);
        });
        if (appTags.size > 0) {
            log('📋 Éléments app-* trouvés : ' + [...appTags].sort().join(', '));
        }

        // Compter les éléments clés
        const nbPselCards = document.querySelectorAll('psel-history-card').length;
        const nbPselEvents = document.querySelectorAll('psel-event-main, psel-event-card, psel-live-event-card').length;
        const nbPselOutcomes = document.querySelectorAll('psel-outcome').length;
        const nbMatchHome = document.querySelectorAll('.match-home').length;
        const nbOutcomeButtons = document.querySelectorAll('.outcomeButton').length;
        log(`📊 [ancien] cards: ${nbPselCards} | events: ${nbPselEvents} | outcomes: ${nbPselOutcomes}`);
        log(`📊 [nouveau] match-home: ${nbMatchHome} | outcomeButtons: ${nbOutcomeButtons}`);
    }

    // ══════════════════════════════════════════════════════════════════
    //  Point d'entrée
    // ══════════════════════════════════════════════════════════════════
    log('🔍 Détection du type de page…');
    dumpPageStructure();

    // 1. Ancien site : page "Mes paris"
    const paris = extracterMesParis();
    if (paris !== null) {
        log(`✅ Mode "Mes paris" (ancien site) — ${paris.length} paris trouvés`);
        sendParis(paris);
        return;
    }

    // 2. Ancien site : page de cotes
    const cotes = extracterPageCotes();
    const questions = extracterMarchesQuestion();
    if (cotes !== null || questions !== null) {
        const combined = [...(cotes || []), ...(questions || [])];
        const nbCotes = cotes ? cotes.length : 0;
        const nbQ = questions ? questions.length : 0;
        log(`✅ ${nbCotes} cotes classiques + ${nbQ} marchés question (ancien site) — ${combined.length} total`);
        sendParis(combined);
        return;
    }

    // 3. Nouveau site 2026 : div.match-home
    const nouveauSite = extracterNouveauSite();
    if (nouveauSite !== null) {
        log(`✅ Mode nouveau site — ${nouveauSite.length} paris trouvés`);
        sendParis(nouveauSite);
        return;
    }

    log('⚠️ Aucune donnée reconnue sur cette page. Es-tu bien sur une page ParionsSport avec des paris ou des cotes ?');

})();
