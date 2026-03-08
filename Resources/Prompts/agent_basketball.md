═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

Recherche web OBLIGATOIRE pour chaque section. Source requise.
Si introuvable : [DONNEE MANQUANTE — Confiance reduite].

1.1 CONTEXTE DU MATCH
Enjeu : [playoffs, play-in, tanking, saison reguliere]
Phase saison : [pre All-Star, post All-Star, playoffs]
Back-to-back : [A: O/N | B: O/N] — si oui, min titulaires la veille

1.2 BLESSURES & ABSENCES — FACTEUR CRITIQUE N°1
→ ESPN NBA Injuries > rotowire.com > CBS Sports
| Joueur | Statut     | Role (Star/Tit/Rot) | BPM   | On/Off diff |
|--------|------------|---------------------|-------|-------------|
Star absent : malus -3 a -8 pts sur note globale selon BPM.
Mouvement de ligne >10 centimes = possible info blessure non publique.

1.3 STATS AVANCEES (saison)
→ nba.com/stats + basketball-reference.com
| Metrique          | Equipe A | Rang NBA | Equipe B | Rang NBA |
|-------------------|----------|----------|----------|----------|
| ORTG              |          |          |          |          |
| DRTG              |          |          |          |          |
| Net Rating        |          |          |          |          |
| Pace              |          |          |          |          |
| eFG%              |          |          |          |          |
| TOV%              |          |          |          |          |
| OREB%             |          |          |          |          |
| FT Rate           |          |          |          |          |
| Points in Paint   |          |          |          |          |
| 3PT%              |          |          |          |          |
| Opp 3PT%          |          |          |          |          |

Les "Four Factors" (eFG%, TOV%, OREB%, FT Rate) = meilleurs predicteurs.

1.4 FORME RECENTE (10 derniers matchs)
→ basketball-reference.com + sofascore.com
| Donnee                    | Equipe A      | Equipe B      |
|---------------------------|---------------|---------------|
| Bilan W-L (total)         |               |               |
| Bilan dom/ext             |               |               |
| Net Rating L10            |               |               |
| Qualite adversaires L10   |               |               |
| Tendance (↑ → ↓)         |               |               |

1.5 JOUEURS CLES (3-4 par equipe)
→ basketball-reference.com
| Joueur | PPG | RPG | APG | TS% | BPM | Forme L5 |
|--------|-----|-----|-----|-----|-----|----------|

1.6 H2H (5 derniers)
→ basketball-reference.com H2H
| Date | Lieu | Score | Absences notables | Coeff recence |
|------|------|-------|-------------------|---------------|
| | | | | <6m=1.0 / 6-12=0.7 / 12-18=0.5 / >18=0.3 |

1.7 FATIGUE & CALENDRIER
| Facteur                    | Equipe A | Equipe B |
|----------------------------|----------|----------|
| Matchs en 7 jours         |          |          |
| Back-to-back               |          |          |
| Repos avant ce match (j)  |          |          |
| Distance parcourue (7j)   |          |          |
B2B = -1.5 a -2 pts de Net Rating en moyenne.

1.8 COTES MARCHE
→ oddsportal.com (Pinnacle = reference sharp)
| Book        | Cote A | Cote B |
|-------------|--------|--------|
| ParionsSport|        |        |
| Pinnacle    |        |        |
| Betclic     |        |        |
Mouvement de ligne : ___
Reverse line movement (public vs sharp) : ___

▶ STOP — Affiche TOUT le contenu ci-dessus AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ANALYSE MATHEMATIQUE (tous les calculs visibles)
═══════════════════════════════════════════════════════

2.1 NOTATION MULTICRITERES (0-100)
Echelle : 90-100=elite top3 | 75-89=tres bon top10 | 60-74=au-dessus moy
         50-59=moyenne | 40-49=en dessous | 25-39=faible | <25=catastrophique

| # | Critere                    | Poids  | Note A | Note B | Justification |
|---|----------------------------|--------|--------|--------|---------------|
| 1 | H2H pondere                | 8%     |        |        |               |
| 2 | Forme recente (10 matchs)  | 18%    |        |        |               |
| 3 | Stats offensives (ORTG, FF)| 15%    |        |        |               |
| 4 | Stats defensives (DRTG)    | 15%    |        |        |               |
| 5 | Impact blessures           | 18%    |        |        |               |
| 6 | Domicile / Exterieur       | 8%     |        |        |               |
| 7 | Dynamique / Elo            | 5%     |        |        |               |
| 8 | Fatigue / calendrier       | 8%     |        |        |               |
| 9 | Signaux marche (sharp $)   | 5%     |        |        |               |
|   | TOTAL                      | 100%   |        |        |               |

Ajustements contextuels :
- Playoffs : Blessures→22%, Forme→12%, Marche→8%
- B2B confirme : Fatigue→14%, Blessures→20%, Forme→14%
- Tanking avere : Blessures→25%, Forme→10%

2.2 PROBABILITES
Score_A = sum(poids_i * note_A_i) / 100 = ___
Score_B = sum(poids_i * note_B_i) / 100 = ___
P_brute(A) = Score_A / (Score_A + Score_B) = ___%

Calibration domicile : si A a domicile → P(A) += 3% (avantage terrain NBA ~+2-3pts)
P(A) = ___% | P(B) = ___%

Cross-validation (si dispo) : ESPN BPI = ___% | Elo = ___%
Si ecart > 8% avec les 3 refs → revoir l'analyse.

2.3 EV & COMPARAISON MARCHE
Marge book = (1/cote_A + 1/cote_B) - 1 = ___%
P_nette(A) = (1/cote_A) / (1/cote_A + 1/cote_B)
EV(A) = (P_simulee(A) * cote_A) - 1

| Equipe | Cote  | P.impl | P.nette | P.sim  | Ecart  | EV     |
|--------|-------|--------|---------|--------|--------|--------|
| A      |       |   %    |    %    |   %    | +/- %  |        |
| B      |       |   %    |    %    |   %    | +/- %  |        |

2.4 VERDICT
| Ecart              | Verdict                  |
|---------------------|--------------------------|
| > +8%               | VALUE BET FORTE ***      |
| +5% a +8%           | VALUE BET CONFIRMEE **   |
| +2% a +5%           | VALUE BET LEGERE *       |
| 0 a +2%             | ZONE NEUTRE              |
| < 0                 | PAS DE VALUE             |

Kelly/4 = (P_sim * cote - 1) / (cote - 1) / 4 = ___% bankroll
Max 2.5% bankroll par pari.

Confiance (/20) :
| Critere                          | Score |
|----------------------------------|-------|
| Qualite donnees (stats avancees) | /5    |
| Completude (blessures confirmees)| /5    |
| Coherence avec BPI/Elo           | /5    |
| Coherence consensus marche       | /5    |
TOTAL : ___/20 (16-20=elevee, 12-15=moderee, <12=faible)

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B]                                ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/20                    ║
║ FACTEURS CLES : 1. ___ 2. ___ 3. ___             ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
