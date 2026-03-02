/**
 * flashscore_extractor.js — Version WebView2
 * Le placeholder __BET_B64__ est remplacé par ExtractionService.cs
 * avant l'injection avec les données du pari en base64 UTF-8.
 * Envoie les stats via window.chrome.webview.postMessage (FS_STATS).
 */
(function () {
    'use strict';

    var BET_B64 = '__BET_B64__';

    function log(msg) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify({ type: 'LOG', message: msg }));
        }
    }

    function sendStats(data) {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(JSON.stringify({ type: 'FS_STATS', data: data }));
        }
    }

    // ── Décodage des données du pari ──────────────────────────────────────
    var BET;
    try {
        var decoded = decodeURIComponent(escape(atob(BET_B64)));
        BET = JSON.parse(decoded);
    } catch (e) {
        log('❌ Erreur décodage BET: ' + e.message);
        return;
    }

    // ── Détection page de match (pas page de recherche) ───────────────────
    var url = location.href;
    if (url.indexOf('/search/') !== -1 || url === 'https://www.flashscore.com/') {
        log('⚠️ Navigue vers la page du match FlashScore, puis clique "Injecter le script"');
        return;
    }

    // ── Extraction du score ───────────────────────────────────────────────
    var S = { score: null, minute: null, rows: [] };

    var scoreEls = document.querySelectorAll('.detailScore__value');
    if (scoreEls.length >= 2) {
        S.score = scoreEls[0].textContent.trim() + ' - ' + scoreEls[1].textContent.trim();
    }

    // ── Extraction de la minute / statut ──────────────────────────────────
    var timeSelectors = [
        '.fixedHeaderDuel__detailStatus',
        '.detailScore__status',
        '[class*="matchStatus"]',
        '.event__stage--block',
        '[class*="detailScore__status"]'
    ];
    for (var i = 0; i < timeSelectors.length; i++) {
        var tel = document.querySelector(timeSelectors[i]);
        if (tel && tel.textContent.trim()) {
            S.minute = tel.textContent.trim();
            break;
        }
    }

    // ── Extraction des stats (tableau possession, tirs, xG…) ─────────────
    var rowSelectors = [
        '.stat__row',
        '[class*="statistic__row"]',
        '[class*="stat__"]',
        '.matchInfoRow'
    ];

    var foundRows = false;
    for (var rs = 0; rs < rowSelectors.length && !foundRows; rs++) {
        var rowEls = document.querySelectorAll(rowSelectors[rs]);
        rowEls.forEach(function (r) {
            var n = r.querySelector('[class*="categoryName"],[class*="statName"],[class*="statCategory"]');
            var h = r.querySelector('[class*="homeValue"],[class*="homeVal"],[class*="home"]');
            var a = r.querySelector('[class*="awayValue"],[class*="awayVal"],[class*="away"]');
            if (n && h && a && n !== h && n !== a) {
                var nameText = n.textContent.trim();
                var homeText = h.textContent.trim();
                var awayText = a.textContent.trim();
                // Filtre les valeurs vides ou identiques (évite les faux positifs)
                if (nameText && homeText && awayText && homeText !== awayText) {
                    S.rows.push({ name: nameText, home: homeText, away: awayText });
                    foundRows = true;
                }
            }
        });
    }

    log('FlashScore extrait : score=' + (S.score || 'N/A') +
        ' min=' + (S.minute || 'N/A') +
        ' stats=' + S.rows.length + ' ligne(s)');

    sendStats(S);

})();
