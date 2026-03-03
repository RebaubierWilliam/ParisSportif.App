/**
 * flashscore_extractor.js — Version WebView2 v3
 * Le placeholder __BET_B64__ est remplacé par ExtractionService.cs
 * avant l'injection avec les données du pari en base64 UTF-8.
 * Envoie les stats via window.chrome.webview.postMessage (FS_STATS).
 *
 * Stratégies d'extraction :
 *  1. Sélecteurs CSS (multiples fallbacks)
 *  2. Structure 3-colonnes (home | name | away)
 *  3. Clic automatique sur l'onglet "Statistics" + retry après 1.5s
 */
(function () {
    'use strict';

    var BET_B64 = '__BET_B64__';

    function log(msg) {
        if (window.chrome && window.chrome.webview)
            window.chrome.webview.postMessage(JSON.stringify({ type: 'LOG', message: msg }));
    }

    function sendStats(data) {
        if (window.chrome && window.chrome.webview)
            window.chrome.webview.postMessage(JSON.stringify({ type: 'FS_STATS', data: data }));
    }

    // ── Décodage des données du pari ──────────────────────────────────────
    try {
        var bytes = Uint8Array.from(atob(BET_B64), function (c) { return c.charCodeAt(0); });
        JSON.parse(new TextDecoder('utf-8').decode(bytes)); // validation uniquement
    } catch (e) {
        log('❌ Erreur décodage BET: ' + e.message);
        return;
    }

    // ── Détection page de match (pas page de recherche) ───────────────────
    var url = location.href;
    if (url.indexOf('/search/') !== -1 ||
        url === 'https://www.flashscore.com/' ||
        url === 'https://www.flashscore.fr/') {
        log('⚠️ Navigue vers la page du match FlashScore puis clique "Injecter le script"');
        return;
    }

    var S = { score: null, minute: null, rows: [] };

    // ── Extraction du score ───────────────────────────────────────────────
    function extractScore() {
        var scoreSelectors = [
            '.detailScore__value',
            '[class*="detailScore__value"]',
            '[class*="score__value"]',
            '[class*="scoreValue"]',
            '[class*="Score__value"]',
            '[class*="scoreboard"]',
        ];

        for (var i = 0; i < scoreSelectors.length; i++) {
            var els = document.querySelectorAll(scoreSelectors[i]);
            if (els.length >= 2) {
                var v1 = els[0].textContent.trim();
                var v2 = els[1].textContent.trim();
                if (/^\d+$/.test(v1) && /^\d+$/.test(v2)) {
                    S.score = v1 + ' - ' + v2;
                    return;
                }
            }
        }

        // Fallback structurel : deux éléments consécutifs contenant uniquement un chiffre
        var allSpans = Array.from(document.querySelectorAll('span, div'))
            .filter(function (el) {
                var t = el.textContent.trim();
                return /^\d{1,2}$/.test(t) && el.children.length === 0;
            });
        if (allSpans.length >= 2) {
            S.score = allSpans[0].textContent.trim() + ' - ' + allSpans[1].textContent.trim();
        }
    }

    // ── Extraction de la minute / statut ──────────────────────────────────
    function extractMinute() {
        var timeSelectors = [
            '.fixedHeaderDuel__detailStatus',
            '[class*="detailStatus"]',
            '[class*="matchStatus"]',
            '[class*="fixedHeader"]',
            '[class*="eventStage"]',
            '[class*="stage--block"]',
            '[class*="statusWrapper"]',
            '[class*="liveTime"]',
            '[class*="matchTime"]',
            '[class*="gameTime"]',
            '[class*="clock"]',
            '[class*="minute"]',
        ];

        for (var i = 0; i < timeSelectors.length; i++) {
            var el = document.querySelector(timeSelectors[i]);
            if (el) {
                var text = el.textContent.trim();
                if (text && text.length > 0 && text.length < 20) {
                    S.minute = text;
                    return;
                }
            }
        }
    }

    // ── Clic sur l'onglet Statistiques ───────────────────────────────────
    function tryClickStatsTab() {
        var keywords = ['statistics', 'statistiques', 'stats'];

        // Chercher par texte exact dans liens / boutons / onglets
        var candidates = document.querySelectorAll('a, button, [role="tab"], li');
        for (var i = 0; i < candidates.length; i++) {
            var el = candidates[i];
            var text = el.textContent.trim().toLowerCase();
            if (keywords.indexOf(text) !== -1) {
                el.click();
                return true;
            }
        }

        // Chercher par href
        var statsLink = document.querySelector(
            'a[href*="statistics"], a[href*="statistiques"], [data-tabid="statistics"]'
        );
        if (statsLink) {
            statsLink.click();
            return true;
        }

        return false;
    }

    // ── Extraction des statistiques (multi-stratégie) ─────────────────────
    function extractStats() {
        var rows = [];

        // ─ Stratégie 1 : sélecteurs CSS (BEM FlashScore historiques + variantes) ─
        var rowSelectors = [
            '.stat__row',
            '[class*="stat__row"]',
            '[class*="statistic__row"]',
            '[class*="statistics__row"]',
            '[class*="statRow"]',
            '[class*="StatRow"]',
            '[class*="matchStat"]',
            '[class*="matchInfoRow"]',
            '[class*="wcl-statistics"]',
        ];

        for (var rs = 0; rs < rowSelectors.length && rows.length === 0; rs++) {
            var rowEls = document.querySelectorAll(rowSelectors[rs]);
            rowEls.forEach(function (r) {
                // Chercher nom + valeurs home/away dans des sous-éléments
                var n = r.querySelector(
                    '[class*="categoryName"],[class*="statName"],[class*="statCategory"],' +
                    '[class*="category"],[class*="label"],[class*="name"]'
                );
                var h = r.querySelector(
                    '[class*="homeValue"],[class*="homeVal"],[class*="home"],[class*="left"],[class*="Home"]'
                );
                var a = r.querySelector(
                    '[class*="awayValue"],[class*="awayVal"],[class*="away"],[class*="right"],[class*="Away"]'
                );

                if (n && h && a && n !== h && n !== a) {
                    var nameText = n.textContent.trim();
                    var homeText = h.textContent.trim();
                    var awayText = a.textContent.trim();
                    if (nameText && homeText && awayText && homeText !== awayText && nameText.length < 40) {
                        rows.push({ name: nameText, home: homeText, away: awayText });
                    }
                }
            });
        }

        // ─ Stratégie 2 : structure 3-colonnes ──────────────────────────────
        // Pattern : [valeur_home (courte)] [nom_stat (textuel)] [valeur_away (courte)]
        if (rows.length === 0) {
            var seen = new Set();
            var allDivs = document.querySelectorAll('div, section, tr');

            for (var i = 0; i < allDivs.length; i++) {
                var children = Array.from(allDivs[i].children);
                if (children.length !== 3) continue;

                var left  = children[0].textContent.trim();
                var mid   = children[1].textContent.trim();
                var right = children[2].textContent.trim();

                // Filtres : valeurs numériques/pourcentages courtes + nom texte
                if (!left || !mid || !right) continue;
                if (left === mid || mid === right) continue;
                if (left.length > 8 || right.length > 8) continue;
                if (mid.length < 3 || mid.length > 40) continue;
                if (seen.has(mid)) continue;
                if (!/^[\d%., \/\-]+$/.test(left) || !/^[\d%., \/\-]+$/.test(right)) continue;

                seen.add(mid);
                rows.push({ name: mid, home: left, away: right });
            }
        }

        // ─ Stratégie 3 : chercher des noms de stats connus et remonter au parent ─
        if (rows.length === 0) {
            var knownStats = [
                'Ball Possession', 'Possession', 'Shots on Target', 'Tirs cadrés',
                'Total Shots', 'Tirs totaux', 'Corners', 'Yellow Cards', 'Cartons jaunes',
                'xG', 'Attacks', 'Dangerous Attacks', 'Free Kicks', 'Coups francs',
                'Offsides', 'Hors-jeux', 'Saves', 'Arrêts',
            ];

            var allText = document.querySelectorAll('span, div, td');
            for (var j = 0; j < allText.length; j++) {
                var el = allText[j];
                var t = el.textContent.trim();
                if (knownStats.indexOf(t) === -1) continue;
                // Remonter pour trouver le parent qui a les valeurs
                var parent = el.parentElement;
                for (var depth = 0; depth < 3 && parent; depth++) {
                    var pChildren = Array.from(parent.children);
                    if (pChildren.length >= 2) {
                        var homeEl = pChildren[0];
                        var awayEl = pChildren[pChildren.length - 1];
                        var hv = homeEl.textContent.trim();
                        var av = awayEl.textContent.trim();
                        if (hv && av && hv !== av && hv !== t && av !== t) {
                            rows.push({ name: t, home: hv, away: av });
                            break;
                        }
                    }
                    parent = parent.parentElement;
                }
            }
        }

        S.rows = rows;
        return rows.length > 0;
    }

    // ── Exécution principale ──────────────────────────────────────────────
    extractScore();
    extractMinute();
    var statsFound = extractStats();

    log('📊 FlashScore : score=' + (S.score || 'N/A') +
        '  min=' + (S.minute || 'N/A') +
        '  stats=' + S.rows.length + ' ligne(s)');

    if (!statsFound) {
        // Tenter clic sur l'onglet Statistiques
        var clicked = tryClickStatsTab();
        if (clicked) {
            log('🔄 Onglet Statistics cliqué — ré-extraction dans 1.5s…');
            setTimeout(function () {
                extractStats();
                log('📊 Après onglet : ' + S.rows.length + ' ligne(s)');
                sendStats(S);
            }, 1500);
            return; // sendStats appelé par setTimeout
        } else {
            log('⚠️ 0 stats trouvées. Va sur l\'onglet "Statistics" de FlashScore puis clique à nouveau sur "Injecter".');
        }
    }

    sendStats(S);

})();
