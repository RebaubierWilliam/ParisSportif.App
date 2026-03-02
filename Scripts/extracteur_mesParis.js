/**
 * extracteur_mesParis.js — Version WebView2
 * Remplace alert/window.open/clipboard par window.chrome.webview.postMessage()
 * Injecté automatiquement par ExtractionService.cs sur la page "Mes paris"
 */
(function () {
    'use strict';

    function log(msg) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify({ type: 'LOG', message: msg }));
        }
    }

    function sendParis(paris) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify({ type: 'PARIS_EXTRACTED', data: paris }));
        }
    }

    // ── Détection du sport ─────────────────────────────────────────────────
    function detecterSportClasses(classes) {
        if (classes.includes('football'))   return 'Football';
        if (classes.includes('tennis'))     return 'Tennis';
        if (classes.includes('basketball')) return 'Basketball';
        if (classes.includes('handball'))   return 'Handball';
        if (classes.includes('volleyball')) return 'Volley Ball';
        if (classes.includes('snooker'))    return 'Snooker';
        if (classes.includes('rugby'))      return 'Rugby';
        if (classes.includes('hockey'))     return 'Hockey';
        return 'Autre';
    }

    // ── Extraction d'un pari depuis une carte historique ──────────────────
    function extracterPariDepuisCard(card) {
        const p = {};

        // Type
        const titleEl = card.querySelector('.psel-history-card__title');
        p.type = titleEl ? titleEl.textContent.trim() : 'Simple';

        // Sport
        const iconEl = card.querySelector('[class*="sprite-icon-"]');
        p.sport = iconEl ? detecterSportClasses(iconEl.className) : 'Autre';

        // Équipes
        const opps = card.querySelectorAll('.psel-opponent__name');
        p.equipes = [];
        opps.forEach((o, i) => { if (i < 2) p.equipes.push(o.textContent.trim()); });

        // Score (live uniquement)
        const scoreEl = card.querySelector('.psel-score--current');
        p.score = scoreEl ? scoreEl.textContent.replace(/\s+/g, ' ').trim() : null;

        // Statut live
        const liveTimerEl  = card.querySelector('psel-live-timer');
        const marketLabelEl = card.querySelector('.psel-cart-market__label');
        const marketLabel  = marketLabelEl ? marketLabelEl.textContent.replace(/\s+/g, ' ').trim() : '';
        p.isLive = !!(liveTimerEl || marketLabel.includes('Live'));

        // Minute / heure
        const timerEl = card.querySelector('.psel-timer');
        p.minuteOuHeure = timerEl ? timerEl.textContent.replace(/\s+/g, ' ').trim() : 'N/A';

        // Marché et sélection
        p.marche = marketLabel;
        const selEl = marketLabelEl ? marketLabelEl.querySelector('.psel-font-bold') : null;
        p.selection = selEl ? selEl.textContent.trim() : 'N/A';

        // Cote
        const coteEl = card.querySelector('.psel-cart-market__value');
        p.cote = coteEl ? coteEl.textContent.trim() : 'N/A';

        // Résumé financier
        p.coteTotale = 'N/A'; p.mise = 'N/A'; p.gainsPotentiels = 'N/A';
        card.querySelectorAll('.psel-history-card__summary__item').forEach(item => {
            const lbl = item.querySelector('.psel-text-sm');
            const val = item.querySelector('.psel-history-card__summary__item__amount');
            if (!lbl || !val) return;
            const t = lbl.textContent.trim();
            if (t.includes('Cote totale')) p.coteTotale      = val.textContent.trim();
            if (t.includes('Mise'))        p.mise             = val.textContent.trim();
            if (t.includes('Gains'))       p.gainsPotentiels  = val.textContent.trim();
        });

        // Cashout
        const cashoutBtn = card.querySelector('.psel-button--cashout');
        if (cashoutBtn) {
            const txt = cashoutBtn.textContent.replace(/\s+/g, ' ').trim();
            const m   = txt.match(/Cash\s*Out\s*[:\s]+([\d,]+\s*€)/i);
            p.cashout = m ? m[1].trim() : txt.replace(/Cash\s*Out\s*:/i, '').trim();
        } else {
            p.cashout = null;
        }

        // Numéro & date
        p.numeroPari = 'N/A'; p.datePari = 'N/A';
        const infoEl = card.querySelector('.psel-history-card__info');
        if (infoEl) {
            infoEl.querySelectorAll('span').forEach(span => {
                const t = span.textContent.trim();
                if (t.startsWith('Pari')) p.numeroPari = t;
                if (t.startsWith('Le ')) p.datePari   = t;
            });
        }

        return p;
    }

    // ── Point d'entrée ────────────────────────────────────────────────────
    const mainElement = document.querySelector('main');
    if (!mainElement) {
        log('⚠️ Aucun élément <main> trouvé');
        return;
    }

    const historyCards = mainElement.querySelectorAll('psel-history-card');
    if (historyCards.length === 0) {
        log('⚠️ Aucune psel-history-card trouvée. Es-tu bien sur la page "Mes paris" ?');
        return;
    }

    const mesParis = [];
    historyCards.forEach(card => {
        try { mesParis.push(extracterPariDepuisCard(card)); }
        catch (err) { log('Erreur parsing card: ' + err.message); }
    });

    log('Extraction terminée : ' + mesParis.length + ' paris');
    sendParis(mesParis);

})();
