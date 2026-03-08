═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

Recherche web OBLIGATOIRE pour chaque section. Source requise.
Adapte les criteres au sport concerne.
Si introuvable : [DONNEE MANQUANTE — Confiance reduite].

1.1 CONTEXTE GENERAL
Competition : [Nom, phase (saison reguliere, playoffs, coupe)]
Enjeu : [Titre, qualification, maintien, autre]
Conditions : [Lieu, meteo/surface si pertinent]

1.2 DISPONIBILITE EFFECTIFS — FACTEUR CLE
→ transfermarkt.com > sofascore.com > "[equipe] injuries [date]"
| Joueur | Statut | Role (Star/Tit/Rot) | Impact estime |
|--------|--------|---------------------|---------------|

1.3 FORME RECENTE (5-10 derniers matchs)
→ sofascore.com ou flashscore.com
| Donnee                    | Option A       | Option B       |
|---------------------------|----------------|----------------|
| Bilan W-L (global)        |                |                |
| Bilan dom/ext             |                |                |
| Pts/buts marques (moy)    |                |                |
| Pts/buts encaisses (moy)  |                |                |
| Qualite adversaires       |                |                |

1.4 STATS AVANCEES (saison)
Adapte au sport. Pour chaque stat cle, indique le rang relatif.
| Stat cle du sport        | Option A | Rang | Option B | Rang |
|--------------------------|----------|------|----------|------|

1.5 H2H (5 derniers)
→ sofascore.com > flashscore.com
| Date | Score | Lieu | Coeff recence |
|------|-------|------|---------------|
| | | | <6m=1.0 / 6-12=0.8 / 12-24=0.6 / >24=0.4 |

1.6 FACTEURS CONTEXTUELS [-2 a +2]
| Facteur          | A   | B   | Justification |
|------------------|-----|-----|---------------|
| Enjeu            |     |     |               |
| Fatigue/calendrier|    |     |               |
| Momentum         |     |     |               |
| Domicile/Conditions|   |     |               |

1.7 COTES MARCHE
→ oddsportal.com
| Book        | Cote A | Cote B | (Cote N si applicable) |
|-------------|--------|--------|------------------------|
| ParionsSport|        |        |                        |
| Pinnacle    |        |        |                        |
| Betclic     |        |        |                        |

▶ STOP — Affiche TOUT le contenu ci-dessus AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ANALYSE MATHEMATIQUE (tous les calculs visibles)
═══════════════════════════════════════════════════════

2.1 NOTATION MULTICRITERES (0-100)
| # | Critere                | Poids  | Note A | Note B | Justification |
|---|------------------------|--------|--------|--------|---------------|
| 1 | H2H pondere            | 10%    |        |        |               |
| 2 | Forme recente          | 20%    |        |        |               |
| 3 | Stats offensives       | 15%    |        |        |               |
| 4 | Stats defensives       | 15%    |        |        |               |
| 5 | Impact blessures       | 15%    |        |        |               |
| 6 | Domicile/exterieur     | 10%    |        |        |               |
| 7 | Dynamique/confiance    | 5%     |        |        |               |
| 8 | Fatigue/calendrier     | 5%     |        |        |               |
| 9 | Conditions/contexte    | 5%     |        |        |               |
|   | TOTAL                  | 100%   |        |        |               |

2.2 PROBABILITES
Score_A = sum(poids_i * note_A_i) / 100 = ___
Score_B = sum(poids_i * note_B_i) / 100 = ___
P(A) = Score_A / (Score_A + Score_B) = ___%
P(B) = 1 - P(A) = ___%

2.3 EV & COMPARAISON MARCHE
Marge book = sum(1/cotes) - 1 = ___%
P_nette = P_implicite / sum(P_implicites)
EV = (P_simulee * Cote) - 1

| Option | Cote  | P.impl | P.nette | P.sim  | Ecart  | EV     |
|--------|-------|--------|---------|--------|--------|--------|
| A      |       |   %    |    %    |   %    | +/- %  |        |
| B      |       |   %    |    %    |   %    | +/- %  |        |

2.4 VERDICT
| Ecart              | Verdict                  |
|---------------------|--------------------------|
| > +5%               | VALUE BET FORTE ***      |
| +2% a +5%           | VALUE BET CONFIRMEE **   |
| 0 a +2%             | ZONE NEUTRE              |
| < 0                 | PAS DE VALUE             |

Confiance (/20) :
| Critere                  | Score |
|--------------------------|-------|
| Qualite donnees          | /5    |
| Completude               | /5    |
| Coherence H2H            | /5    |
| Coherence marche          | /5    |
TOTAL : ___/20 (16-20=elevee, 12-15=moderee, <12=faible)

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B]                                ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/20                    ║
║ FACTEURS CLES : 1. ___ 2. ___                    ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
