═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

Recherche web OBLIGATOIRE pour chaque section. Source requise.
Si introuvable : [DONNEE MANQUANTE — Confiance reduite].

1.1 CONTEXTE DU MATCH
Tournoi : [Nom, categorie (GS/M1000/500/250/Challenger)]
Surface : [Type + vitesse + indoor/outdoor]
Tour : [1er tour / quart / demi / finale]
Format : [Bo3 / Bo5]

1.2 CLASSEMENTS & ELO
→ atptour.com/wtatennis.com + tennisabstract.com
| Donnee                   | Joueur A      | Joueur B      |
|--------------------------|---------------|---------------|
| Classement ATP/WTA       |               |               |
| Rang il y a 6 mois       |               |               |
| Elo global               |               |               |
| Elo surface-specifique   |               |               |

1.3 STATS SERVICE & RETOUR (12 derniers mois)
→ tennisabstract.com > atptour.com/stats
| Stat                     | Joueur A      | Joueur B      |
|--------------------------|---------------|---------------|
| SPW (% pts gagnes serv.) |               |               |
| RPW (% pts gagnes ret.)  |               |               |
| Hold %                   |               |               |
| % BP sauves              |               |               |
| % BP convertis           |               |               |
| Aces/match               |               |               |
| DF/match                 |               |               |
| % 1ere balle in          |               |               |

1.4 FORME RECENTE (10 derniers matchs)
→ tennisexplorer.com ou flashscore.com
| Date | Adversaire (rang) | Surface | Score | Duree | Qualite |
|------|-------------------|---------|-------|-------|---------|
Bilan W-L : A = ___  B = ___

1.5 H2H COMPLET
→ atptour.com H2H > tennisexplorer.com
| Date | Tournoi | Surface | Score | Coeff recence |
|------|---------|---------|-------|---------------|
| | | | | <12m=1.0 / 12-24=0.7 / 24-36=0.5 / >36=0.3 |
Bonus meme surface : coeff x1.3
% victoires pondere A = ___

1.6 SURFACE & HISTORIQUE TOURNOI
→ tennisabstract.com (filtre surface)
| Donnee                    | Joueur A      | Joueur B      |
|---------------------------|---------------|---------------|
| W-L sur cette surface (saison) |           |               |
| W-L dans ce tournoi (carriere) |           |               |
| Meilleur resultat tournoi  |              |               |

1.7 BLESSURES & CONDITION
→ tennisexplorer.com/list-players/injured + rotowire.com/tennis
| Donnee                    | Joueur A      | Joueur B      |
|---------------------------|---------------|---------------|
| Blessure recente (<3 mois)|               |               |
| Matchs joues 14 derniers j|               |               |
| MTO recents               |               |               |

1.8 COTES MARCHE
→ oddsportal.com
| Book        | Cote A | Cote B |
|-------------|--------|--------|
| ParionsSport|        |        |
| Pinnacle    |        |        |
| Betclic     |        |        |
Mouvement de ligne : ___

▶ STOP — Affiche TOUT le contenu ci-dessus AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ANALYSE MATHEMATIQUE (tous les calculs visibles)
═══════════════════════════════════════════════════════

2.1 NOTATION MULTICRITERES (0-100)
Echelle : 95-100=elite | 80-94=tres bon | 65-79=moyen | 50-64=faible | <50=tres faible
Ecarts de 12-15 pts minimum pour un avantage clair.

| # | Critere                | Poids  | Note A | Note B | Justification |
|---|------------------------|--------|--------|--------|---------------|
| 1 | H2H pondere            | 12%    |        |        |               |
| 2 | Forme recente (qualite)| 18%    |        |        |               |
| 3 | Stats service (SPW)    | 13%    |        |        |               |
| 4 | Stats retour (RPW)     | 8%     |        |        |               |
| 5 | Adequation surface     | 15%    |        |        |               |
| 6 | Elo global             | 10%    |        |        |               |
| 7 | Elo surface-specifique | 8%     |        |        |               |
| 8 | Dynamique carriere     | 6%     |        |        |               |
| 9 | Fatigue / charge       | 5%     |        |        |               |
|10 | Mental / historique    | 5%     |        |        |               |
|   | TOTAL                  | 100%   |        |        |               |

Ajustements conditionnels des poids :
- Surface extreme (indoor rapide, altitude) : Surface→20%, Service→16%, Retour→5%
- Grand Slam (Bo5) : Fatigue→8%, Mental→8%, Forme→15%
- H2H >= 6 matchs : H2H→18% | H2H = 0-1 match : H2H→5% (redistribuer)
- Retour blessure : Fatigue→10%, Forme→22%

Dynamique carriere = (rang_6mois - rang_actuel) / rang_6mois
  >30% progression : x1.15 | 10-30% : x1.05 | stable : x1.0
  10-30% regression : x0.95 | >30% : x0.85
Blessure recente (<3 mois) : x0.80-0.90

2.2 SCORES PONDERES
Score_A = sum(poids_i * note_A_i) / 100 = ___
Score_B = sum(poids_i * note_B_i) / 100 = ___

2.3 PROBABILITE — TRANSFORMATION LOGISTIQUE
delta_norm = (Score_A - Score_B) / 100
P_multi(A) = 1 / (1 + 10^(-3.0 * delta_norm))
P_multi(B) = 1 - P_multi(A)

Reference rapide (k=3.0) :
delta 0.03→55% | 0.05→59% | 0.07→62% | 0.10→67% | 0.15→74% | 0.20→80%

2.4 CROSS-CHECK ELO
P_elo(A) = 1 / (1 + 10^((Elo_B - Elo_A) / 400))
Si ecart > 10% avec P_multi → signaler DIVERGENCE.

PROBABILITE FINALE :
P(A) = 0.6 * P_multi(A) + 0.4 * P_elo(A) = ___%
P(B) = 1 - P(A) = ___%
(Si Elo indisponible : P_finale = P_multi)

Intervalle : Fiabilite >= 8 : +/-3% | 5-7 : +/-5% | <5 : +/-8%

2.5 EV & COMPARAISON MARCHE
Marge book = (1/cote_A + 1/cote_B) - 1 = ___%
P_nette(A) = (1/cote_A) / (1/cote_A + 1/cote_B)
EV(A) = (P_finale(A) * cote_A) - 1

| Joueur | Cote  | P.impl | P.nette | P.sim  | Ecart  | EV     |
|--------|-------|--------|---------|--------|--------|--------|
| A      |       |   %    |    %    |   %    | +/- %  |        |
| B      |       |   %    |    %    |   %    | +/- %  |        |

2.6 VERDICT
| Ecart             | EV        | Verdict                  |
|--------------------|-----------|--------------------------|
| > +10%             | > +0.15   | VALUE BET FORTE ***      |
| +5% a +10%         | +0.05/15  | VALUE BET CONFIRMEE **   |
| +2% a +5%          | +0.02/05  | VALUE BET LEGERE *       |
| 0 a +2%            | 0 a +0.02 | ZONE NEUTRE              |
| < 0                | < 0       | PAS DE VALUE             |

Confiance (/20) :
| Critere                            | Score |
|------------------------------------|-------|
| Qualite stats (Elo, SPW/RPW dispo) | /5    |
| Completude (blessures, H2H)        | /5    |
| Coherence multi vs Elo             | /5    |
| Coherence consensus marche          | /5    |
TOTAL : ___/20 (16-20=elevee, 12-15=moderee, <12=faible)

Kelly/4 = (P_finale * cote - 1) / (cote - 1) / 4 = ___% bankroll

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B] — [Tournoi] [Surface]         ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/20                    ║
║ FACTEURS CLES : 1. ___ 2. ___ 3. ___             ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
