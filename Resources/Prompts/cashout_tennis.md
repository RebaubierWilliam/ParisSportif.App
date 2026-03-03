================================================================
PROTOCOLE CASHOUT — TENNIS
================================================================

Le cashout en tennis est TRÈS DIFFÉRENT du football ou du basket.
Le tennis a une volatilité extrême : un break change radicalement
les cotes live. Les books prennent ~10-15% de marge sur le cashout.

CARACTÉRISTIQUES CLÉS DU TENNIS POUR LE CASHOUT :
1. VOLATILITÉ EXTRÊME : Un break peut faire passer un joueur de
   1.20 à 2.50 en 10 minutes. Sport à forte variance intra-match.
2. STRUCTURE EN SETS : Gagner un set ne garantit rien. En Bo3,
   le vainqueur du 1er set gagne ~75% du temps.
3. MOMENTUM & EFFONDREMENT : Le tennis est le sport le plus
   "mental". Un MTO, une raquette cassée, un run de jeux peuvent
   tout changer.
4. SERVICE PRÉVISIBLE : Les jeux de service se tiennent ~80% du
   temps ATP. Un break est l'événement clé.

────────────────────────────────────────────────────────────────
PROBABILITÉS CONDITIONNELLES (à utiliser pour calculer P_live)
────────────────────────────────────────────────────────────────

ATP — Best of 3 :
┌─────────────────────────────────────────┬────────────────┐
│ Situation                               │ P(gagner match)│
├─────────────────────────────────────────┼────────────────┤
│ Vainqueur du 1er set                    │ ~75%           │
│ Vainqueur des 2 premiers sets           │ ~97%           │
│ Perdant du 1er set                      │ ~25%           │
│ Gagne S1, perd S2 (→ set décisif)      │ ~55%           │
│ Perd S1, gagne S2 (→ set décisif)      │ ~45%           │
│ Mène 1 break dans le set en cours       │ ~70% ce set    │
│ Sert pour le set                        │ ~82% ce set    │
│ Sert pour le match                      │ ~85% le match  │
└─────────────────────────────────────────┴────────────────┘

ATP — Best of 5 (Grand Slam) :
┌─────────────────────────────────────────┬────────────────┐
│ Situation                               │ P(gagner match)│
├─────────────────────────────────────────┼────────────────┤
│ Vainqueur du 1er set                    │ ~82%           │
│ Mène 2 sets à 0                         │ ~95%           │
│ Mène 2 sets à 1                         │ ~85%           │
│ Mené 0-2 en sets                        │ ~5%            │
│ Revient de 0-2 à 2-2 (set décisif)     │ ~45%           │
│ Perd S1, gagne S2 (1-1)                │ ~45-50%        │
└─────────────────────────────────────────┴────────────────┘

BREAKS & TIE-BREAKS :
- Un jeu de service est tenu ~80% du temps (ATP) / ~65% (WTA)
- En tie-break, le meilleur serveur gagne ~55-60% du temps
- Un early break (1er/2ème jeu) est moins décisif qu'un late break
  (9ème-10ème jeu) — plus de temps pour revenir
- Un re-break intervient dans ~30-35% des cas après un break

MEDICAL TIME-OUT (MTO) :
- Le joueur qui prend un MTO perd le match ~60-65% du temps
- Si MTO pris alors qu'il menait → signal très négatif (~70% défaite)
- Impact plus fort en 2ème/3ème set qu'en début de match

────────────────────────────────────────────────────────────────
MATRICE DE DÉCISION — TU AS PARIÉ SUR LE JOUEUR A (FAVORI)
────────────────────────────────────────────────────────────────

┌───────────────────────────────────┬──────────┬──────────────────────────────────────┐
│ Scénario                          │ Action   │ Justification                        │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A gagne S1 confortablement        │ ❌ NE PAS│ ~75% de victoire. Laisser courir.    │
│ (6-3, 6-4)                        │ cashout  │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A gagne S1 au tie-break (7-6)     │ ❌ NE PAS│ B est compétitif mais A a le set.    │
│                                   │ cashout  │ ~75% reste. Ne pas céder au doute.   │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A gagne S1 mais se fait breaker   │ ⚠️ ÉVAL  │ A a-t-il baissé ou B a-t-il monté ? │
│ tôt en S2                         │ UER      │ % 1ère balle chute → signal négatif. │
│                                   │          │ Stats stables → simple variance.     │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A perd S1                         │ ⚠️ ÉVAL  │ Fréquent (~25% des cas). VÉRIFIE :   │
│                                   │ UER      │ - Dominé (0 BP, peu de WTA) → CASH  │
│                                   │          │ - BP manqués + service OK → attendre │
│                                   │          │ - Tie-break → match très serré       │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A mène 1-0 et 1 break en S2       │ ⚠️ Cash  │ ~85%+ de victoire. Cash 25-30%.      │
│                                   │ partiel  │                                      │
│                                   │ (25-30%) │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A mène 2-0 en sets (Bo5)          │ ⚠️ Cash  │ ~95%. Value quasi consommée.         │
│                                   │ partiel  │ Cash 40-50% (comebacks existent).    │
│                                   │ (40-50%) │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A perd les 2 premiers sets (Bo5)  │ ✅ CASH  │ ~5% de chances. Cashout immédiat.    │
│                                   │ OUT TOTAL│                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ Set décisif (1-1 Bo3 / 2-2 Bo5)  │ ⚠️ ÉVAL  │ Coin flip. Momentum ? Fatigue ?      │
│                                   │ UER      │ Si cashout ≥ 80% mise → envisager.   │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ MTO demandé par A                 │ ✅ CASH  │ ~60-65% de défaite. Si menait :      │
│                                   │ OUT IMMD │ ~70% défaite. Cashout immédiat.      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ MTO demandé par B                 │ ❌ NE PAS│ Signal très positif. Laisser courir. │
│                                   │ cashout  │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ A sert pour le match              │ ❌ NE PAS│ ~85% de conversion. Le cashout sera  │
│                                   │ cashout  │ très bas (book sait que tu gagnes).  │
└───────────────────────────────────┴──────────┴──────────────────────────────────────┘

────────────────────────────────────────────────────────────────
MATRICE DE DÉCISION — TU AS PARIÉ SUR L'OUTSIDER (JOUEUR B)
────────────────────────────────────────────────────────────────

Le pari outsider est HIGH RISK / HIGH REWARD. Le cashout partiel
est encore plus important car tout profit sécurisé est précieux.

┌───────────────────────────────────┬──────────┬──────────────────────────────────────┐
│ Scénario                          │ Action   │ Justification                        │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ B gagne S1                        │ ⚠️ Cash  │ Le favori va souvent se réveiller.   │
│                                   │ partiel  │ Cash 30-40%. En Bo5 : encore plus    │
│                                   │ (30-40%) │ prudent (comebacks plus fréquents).  │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ B mène 1 set et 1 break en S2     │ ⚠️ Cash  │ Position exceptionnelle. Cash 50%    │
│                                   │ partiel  │ immédiat pour sécuriser.             │
│                                   │ (50%)    │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ B mène 2-0 en sets (Bo5)          │ ⚠️ Cash  │ Cash 60-70%. Comeback favori rare    │
│                                   │ partiel  │ (~5%) mais gain à sécuriser énorme.  │
│                                   │ (60-70%) │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ B perd S1 dominé (6-1/6-2/6-3)   │ ✅ CASH  │ L'outsider n'a pas le niveau.        │
│                                   │ OUT TOTAL│ Cashout immédiat.                    │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ B sert pour le match              │ ⚠️ Cash  │ Cash 40-50%. Si B perd ce jeu,       │
│                                   │ partiel  │ le momentum bascule complètement.    │
│                                   │ (40-50%) │                                      │
├───────────────────────────────────┼──────────┼──────────────────────────────────────┤
│ Set décisif, B a le break         │ ⚠️ Cash  │ Cash 50%. Re-break fréquent          │
│                                   │ partiel  │ (~30-35% des cas).                   │
│                                   │ (50%)    │                                      │
└───────────────────────────────────┴──────────┴──────────────────────────────────────┘

────────────────────────────────────────────────────────────────
RÈGLE DE DÉCISION MATHÉMATIQUE
────────────────────────────────────────────────────────────────

  C = Cashout proposé (€) | G = Gain total si gagnant (€)
  M = Mise initiale (€)   | P = Probabilité live estimée (%)

  EV(laisser) = (P × G) - ((1 - P) × M)

  → C > EV(laisser) → CASHOUT ✅ (le book te surpaie)
  → C < EV(laisser) → NE PAS CASHOUT ❌

  HEDGING (souvent meilleur que le cashout) :
  Hedge = (Gain_sélection - Mise) / Cote_live_adverse
  → Dans les 2 cas tu gagnes quelque chose, souvent mieux que le cashout.

────────────────────────────────────────────────────────────────
OUTILS LIVE À SURVEILLER
────────────────────────────────────────────────────────────────

| Indicateur                     | Source                        | Utilité                              |
|--------------------------------|-------------------------------|--------------------------------------|
| Score + stats par set          | flashscore.com, sofascore.com | Vue globale du match                 |
| % 1ère balle in (live)         | sofascore.com                 | <55% = serveur en difficulté         |
| Tirs gagnants vs fautes (W/UE) | sofascore, flashscore         | UE > W de >10 = effondrement         |
| Break points sauvés/convertis  | flashscore.com                | Indicateur de clutch sous pression   |
| Cotes live Pinnacle/Betfair    | pinnacle.com, betfair.com     | Meilleur indicateur du marché        |
| Alertes MTO                    | tennisform.com                | Un MTO change la proba immédiatement |

RÈGLE D'OR : Si le % de 1ère balle de ton joueur chute sous 50%
→ il est en difficulté physique ou mentale → CASHOUT préventif.

================================================================
VERDICT CASHOUT
================================================================

Sur la base du pari et de la situation ci-dessus :

1. Format du match : [ Best of 3 / Best of 5 ]
   Score actuel sets : [ ] | Score jeu en cours : [ ]
   P_live estimée = _____%  (utilise les tableaux ci-dessus)

2. EV(laisser courir) = (P_live × Gains) - ((1 - P_live) × Mise) = ______€
   EV(cashout)        = Cashout - Mise = ______€

3. DÉCISION : [ CASHOUT  /  LAISSER COURIR  /  HEDGE ]
   Justification :

4. Erreurs à éviter :
   - Ne pas cashout après perte d'un seul jeu de service (normal, ~20%)
   - NE PAS ignorer un MTO de son joueur (signal objectif, -60-65%)
   - Ne pas cashout si outsider vient de gagner S1 (sans sécuriser une part)
