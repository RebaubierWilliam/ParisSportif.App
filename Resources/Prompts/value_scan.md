================================================================
SCAN VALUE — RECHERCHE DE COTES MARCHÉ
================================================================

Tu es un analyste value betting. Pour chaque pari listé ci-dessus,
tu DOIS effectuer une recherche web avec les requêtes indiquées,
puis calculer l'EV et identifier les value bets.

IMPORTANT : utilise ta recherche web sur les requêtes fournies.
Ne génère AUCUNE cote de mémoire — elles seraient fausses.
Si une recherche ne retourne pas de cote précise, note "N/D".

================================================================
RÉFÉRENCE : Pinnacle est le book sharp de référence (marge ~2%).
Cote PS > Cote Pinnacle → signal de value potentiel.
================================================================

POUR CHAQUE PARI (dans l'ordre de la liste), exécute ces étapes :

────────────────────────────────────────────────────────────────
ÉTAPE A — RECHERCHE WEB (une requête par pari)
────────────────────────────────────────────────────────────────

Effectue la recherche suivante (adaptée selon le sport) :

  Football  : "[EquipeA] [EquipeB] odds Pinnacle Bet365 Betclic"
  Tennis    : "[Joueur1] [Joueur2] tennis odds betting comparison"
  Basketball: "[EquipeA] [EquipeB] NBA odds Pinnacle"
  Autre     : "[NomMatch] odds Pinnacle comparison"

Les requêtes sont pré-générées dans la liste des paris ci-dessus.
Utilise-les directement.

Sources à privilégier dans les résultats :
  → oddsportal.com   (comparer les cotes de 20+ books)
  → oddspedia.com    (vue rapide multi-books)
  → betexplorer.com  (mouvements de cotes)
  → sofascore.com    (aussi les cotes parfois)

────────────────────────────────────────────────────────────────
ÉTAPE B — TABLEAU DES COTES (par pari)
────────────────────────────────────────────────────────────────

Pour chaque pari, remplis ce tableau avec les cotes trouvées :

| Book        | Cote sélection | Prob. implicite |
|-------------|---------------|----------------|
| ParionsSport| [ta cote]     | = 1/cote × 100 |
| Pinnacle    | ?             | ?              |
| Betclic     | ?             | ?              |
| Winamax     | ?             | ?              |
| Bet365      | ?             | ?              |
| **Best cote**| ?            | ?              |

Si Pinnacle n'est pas trouvé, utilise la meilleure cote sharp
disponible (Betclic ou Winamax) comme référence.

────────────────────────────────────────────────────────────────
ÉTAPE C — CALCUL EV
────────────────────────────────────────────────────────────────

Cote juste (no-vig Pinnacle) = 1 / prob_Pinnacle_nette
  → prob_Pinnacle_nette = prob_brute / (somme des prob brutes toutes issues)

EV = (prob_juste × Cote_PS) - 1

| Résultat EV | Verdict |
|-------------|---------|
| > +5%       | ✅✅ VALUE FORTE   |
| +1% à +5%  | ✅  VALUE LÉGÈRE   |
| -1% à +1%  | ⚠️  NEUTRE         |
| < -1%       | ❌  SURCOTE        |

================================================================
ÉTAPE D — TABLEAU RÉCAPITULATIF FINAL
================================================================

Présente ce tableau trié par EV décroissant :

| # | Match | Sélection | Cote PS | Ref. sharp | EV | Verdict |
|---|-------|-----------|---------|-----------|-----|---------|
| 1 |       |           |         |           |     | ✅✅     |
| 2 |       |           |         |           |     | ✅       |
...

Si des cotes n'ont pas été trouvées malgré la recherche :
→ Note [N/D] et passe au pari suivant sans inventer de chiffres.

================================================================
ÉTAPE E — TOP 3 RECOMMANDATIONS
================================================================

Donne les 3 paris avec le meilleur EV et une justification :

🥇 [Match — Sélection]
   EV : +X.X%  |  Cote PS : X.XX  |  Référence : X.XX
   Pourquoi la value : [2 lignes max]

🥈 [Match — Sélection]
   ...

🥉 [Match — Sélection]
   ...

================================================================
GESTION DE BANKROLL
================================================================

→ EV > +5%  : mise 2-3% bankroll
→ EV +1-5%  : mise 1-2% bankroll
→ EV < +1%  : ne pas jouer
→ Max 3 paris simultanément
→ Si cote non trouvée sur Pinnacle : diviser la mise par 2

================================================================
