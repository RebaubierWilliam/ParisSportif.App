═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

Recherche web OBLIGATOIRE pour chaque section. Source requise.
Si introuvable : [DONNEE MANQUANTE — Confiance reduite].

1.1 CLASSEMENT & FORME
→ sofascore.com ou fbref.com
| Donnee                    | Equipe A       | Equipe B       |
|---------------------------|----------------|----------------|
| Classement (Pos/Pts/+-)  |                |                |
| Forme 5 derniers (global) |                |                |
| Forme dom/ext (5 dern.)  |                |                |
| Elo (clubelo.com)         |                |                |

1.2 xG & STATS
Source par ligue :
- Top 5 : understat.com → fbref.com
- UEFA : fbref.com (filtre competition) → xgscore.io
- Mineures : fbref.com → footystats.org
- Introuvable : methode classique [signaler]

| Stat                | A (dom) | A (global) | B (ext) | B (global) |
|---------------------|---------|------------|---------|------------|
| Buts marques/m      |         |            |         |            |
| Buts encaisses/m    |         |            |         |            |
| xG pour/m           |         |            |         |            |
| xG contre/m         |         |            |         |            |
| xG L5 (5 derniers)  |         |            |         |            |
| Tirs cadres/m       |         |            |         |            |
| BTTS %              |         |            |         |            |
| Over 2.5 %          |         |            |         |            |

Sources : [URL 1], [URL 2]

1.3 ABSENCES
→ transfermarkt.com > sofascore lineups > "[equipe] injuries [date]"
| Joueur | Poste | Raison | Titulaire ? | Impact xG |
|--------|-------|--------|-------------|-----------|

Regles impact xG :
Buteur principal : -0.15 a -0.30 | DC titulaire : +0.10 a +0.20 adverse
GK titulaire : +0.10 a +0.15 adverse | Milieu createur : -0.10 a -0.20

1.4 H2H (5 derniers)
→ sofascore.com ou flashscore.com
| Date | Score | Lieu | Coeff recence |
|------|-------|------|---------------|
| | | | <6m=1.0 / 6-12=0.8 / 12-24=0.6 / >24=0.4 |

1.5 CONTEXTE [-2 a +2 par facteur]
| Facteur        | A   | B   | Justification |
|----------------|-----|-----|---------------|
| Enjeu          |     |     |               |
| Fatigue/calendrier |  |     |               |
| Momentum       |     |     |               |
| Motivation     |     |     |               |
TOTAL CONTEXTE : A = ___ / B = ___

1.6 MOYENNES LIGUE
→ understat/fbref/footystats (page ligue)
Buts/match global : ___ | Dom : ___ | Ext : ___

1.7 COTES MARCHE
→ oddsportal.com (Pinnacle = reference sharp)
| Book        | Cote 1 | Cote N | Cote 2 |
|-------------|--------|--------|--------|
| ParionsSport|        |        |        |
| Pinnacle    |        |        |        |
| Betclic     |        |        |        |
| Winamax     |        |        |        |

▶ STOP — Affiche TOUT le contenu ci-dessus AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ANALYSE MATHEMATIQUE (tous les calculs visibles)
═══════════════════════════════════════════════════════

2.1 LAMBDA (buts attendus)
Si xG dispo :
  lambda_A = (xG_A_dom + xG_contre_B_ext) / 2
  lambda_B = (xG_B_ext + xG_contre_A_dom) / 2
Si coupe UEFA (<4 matchs en coupe) : lambda = 0.6*lambda_ligue + 0.4*lambda_coupe
Si xG indispo : Force_att * Faiblesse_def * Moy_ligue (signaler)

Ajustements :
| Source            | Regle                          | Valeur |
|-------------------|--------------------------------|--------|
| Absences (1.3)    | Somme impacts xG               | +/-    |
| Momentum >=4V     | x1.05 a x1.10                 | x      |
| Fatigue >=3m/10j  | x0.90 a x0.95                 | x      |
| Contexte (1.5)    | +/-0.03 par pt net d'ecart     | +/-    |
| Derby/finale      | Resserrement lambdas ~5%        | +/-    |

lambda_A ajuste = ___ | lambda_B ajuste = ___

2.2 POISSON + DIXON-COLES
P(k) = (lambda^k * e^-lambda) / k! pour k = 0 a 5
Matrice 6x6 (A en ligne, B en colonne) — afficher les probabilites.
Dixon-Coles : si lambda_A < 1.2 ET lambda_B < 1.2 → rho = -0.10 a -0.15
  → Augmenter P(0-0) et P(1-1) de ~15-20%, reduire P(1-0) et P(0-1)
Sinon rho ≈ 0.

2.3 PROBABILITES 1/N/2
P(A) = sum(A>B) = ___% | P(N) = sum(A=B) = ___% | P(B) = sum(B>A) = ___%
Verification : P(A) + P(N) + P(B) = 100% ✓
Score le plus probable : ___-___ (___%)

2.4 EV & COMPARAISON MARCHE
Marge book = sum(1/cotes) - 1 = ___%
P_nette = P_implicite / sum(P_implicites)
EV = (P_simulee * Cote) - 1

| Issue | Cote  | P.impl | P.nette | P.sim | Ecart  | EV     |
|-------|-------|--------|---------|-------|--------|--------|
| 1 (A) |       |   %    |    %    |   %   | +/- %  |        |
| N     |       |   %    |    %    |   %   | +/- %  |        |
| 2 (B) |       |   %    |    %    |   %   | +/- %  |        |

2.5 SENSIBILITE +/-10%
| Scenario           | lA    | lB    | P(A) | P(N) | P(B) | Value? |
|--------------------|-------|-------|------|------|------|--------|
| Base               |       |       |   %  |   %  |   %  | —      |
| Favorable A (+10%) |       |       |   %  |   %  |   %  | O/N    |
| Defavorable A(-10%)|       |       |   %  |   %  |   %  | O/N    |

Si la value disparait a +/-10% : reduire confiance d'un cran.

2.6 VERDICT
| Ecart (Sim - Marche) | EV        | Verdict                  |
|----------------------|-----------|--------------------------|
| > +10%               | > +0.15   | VALUE BET FORTE ***      |
| +5% a +10%           | +0.05/15  | VALUE BET CONFIRMEE **   |
| +2% a +5%            | +0.02/05  | VALUE BET LEGERE *       |
| 0 a +2%              | 0 a +0.02 | ZONE NEUTRE              |
| < 0                  | < 0       | PAS DE VALUE             |

Confiance (/25) :
| Critere                     | Score |
|-----------------------------|-------|
| Qualite donnees (xG, stats) | /5    |
| Completude (absences, compo)| /5    |
| Stabilite lambda            | /5    |
| Coherence H2H + tendances   | /5    |
| Coherence consensus marche   | /5    |
TOTAL : ___/25 (20-25=elevee, 15-19=moderee, <15=faible)

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B]                                ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/25                    ║
║ FACTEURS CLES : 1. ___ 2. ___ 3. ___             ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
