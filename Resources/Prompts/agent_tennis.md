═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

⚠️ REGLE ABSOLUE : Recherche web OBLIGATOIRE pour CHAQUE section.
Tu dois effectuer AU MINIMUM 10 recherches web distinctes en Phase 1.
Chaque tableau doit contenir des VALEURS CONCRETES (chiffres, scores, dates).
Si introuvable apres 2 sources : [DONNEE MANQUANTE — Confiance reduite].
INTERDICTION d'inventer ou estimer une donnee sans source.

╔═══════════════════════════════════════════════════════════════╗
║ DETECTION NIVEAU DE TOURNOI — Adapte ta strategie            ║
╠═══════════════════════════════════════════════════════════════╣
║ TIER 1 : Grand Slam, Masters 1000, ATP 500                   ║
║   → tennisabstract.com + atptour.com (stats completes, Elo)  ║
║ TIER 2 : ATP 250, WTA 500/250, WTA 1000                      ║
║   → tennisabstract.com + flashscore.com (stats souvent OK)   ║
║ TIER 3 : Challengers, ITF, WTA 125, qualifications           ║
║   → flashscore.com + tennisexplorer.com en priorite           ║
║   → tennisabstract.com (stats partielles pour joueurs <200)  ║
║   → Elo surface-specifique souvent INDISPONIBLE               ║
║   → Accepter une incertitude plus grande                      ║
╚═══════════════════════════════════════════════════════════════╝

SOURCES ALTERNATIVES POUR JOUEURS PEU CONNUS (rang > 150) :
  - flashscore.com : resultats recents + stats de match disponibles
  - tennisexplorer.com : historique complet meme pour joueurs ITF
  - sofascore.com : stats de match (aces, DF, BP) meme en Challenger
  - matchstat.com : stats detaillees pour Challengers et ITF
  - live-tennis.eu : classements live et points en jeu

1.1 CONTEXTE DU MATCH
RECHERCHES A EFFECTUER :
  1. Chercher "[Nom du tournoi] [annee] draw" sur flashscore.com ou atptour.com/wtatennis.com
  2. Chercher "[Nom du tournoi] conditions" pour surface/conditions

Tournoi : [Nom, categorie (GS/M1000/500/250/Challenger)]
Surface : [Type + vitesse estimee (lent/moyen/rapide) + indoor/outdoor]
Tour : [1er tour / quart / demi / finale]
Format : [Bo3 / Bo5]
Conditions : [meteo, altitude, indoor/outdoor]

1.2 CLASSEMENTS & ELO
RECHERCHES A EFFECTUER (toutes obligatoires) :
  TIER 1-2 :
    1. Chercher "[Joueur A] ranking" sur atptour.com (ATP) ou wtatennis.com (WTA)
    2. Chercher "[Joueur B] ranking" idem
    3. Chercher "[Joueur A] elo rating" sur tennisabstract.com/reports/rankingsDetail.html
    4. Chercher "[Joueur B] elo rating" idem
    5. Pour Elo surface-specifique : tennisabstract.com → onglet surface (clay/hard/grass)
  TIER 3 (Challengers, ITF) :
    3b. Si Elo indisponible sur tennisabstract, chercher le classement live
        sur live-tennis.eu/en/atp-live-ranking (ou wta-live-ranking)
    5b. Si Elo surface indisponible : utiliser le W-L sur la surface comme proxy
  DANS TOUS LES CAS :
    6. Chercher "[Joueur A] ranking history" pour evolution 6 mois

| Donnee                   | Joueur A      | Joueur B      | Source |
|--------------------------|---------------|---------------|--------|
| Classement ATP/WTA       |               |               |        |
| Rang il y a 6 mois       |               |               |        |
| Elo global               |               |               |        |
| Elo surface-specifique   |               |               |        |

Sources : [URL 1], [URL 2], ...

1.3 STATS SERVICE & RETOUR (12 derniers mois)
RECHERCHES A EFFECTUER (dans cet ordre) :
  TIER 1-2 :
    1. Chercher "[Joueur A] stats 2024" sur tennisabstract.com/cgi-bin/player-classic.cgi?p=[Joueur]
    2. Chercher "[Joueur B] stats 2024" idem
    3. Filtrer par surface : tennisabstract.com → selectionner surface
    4. Fallback : chercher "[Joueur] statistics 2024-25" sur atptour.com/en/stats/
  TIER 3 (Challengers, ITF) — stats souvent partielles :
    1. Chercher "[Joueur] stats" sur tennisabstract.com (peut etre incomplet)
    2. Chercher "[Joueur] profile" sur flashscore.com → onglet stats
    3. Chercher "[Joueur]" sur matchstat.com (bon pour Challengers)
    4. Fallback : calculer SPW/RPW manuellement a partir des 5-10 derniers matchs
       visibles sur flashscore.com ou tennisexplorer.com (stats par match)
    → Si stats insuffisantes : signaler [STATS PARTIELLES — echantillon = X matchs]
  DANS TOUS LES CAS :
    5. Pour stats sur cette surface specifique : filtrer "surface = [clay/hard/grass]"
    6. Chercher "[Joueur A] serve stats" et "[Joueur B] return stats" pour details

| Stat                     | Joueur A      | Joueur B      | Surface? | Source |
|--------------------------|---------------|---------------|----------|--------|
| SPW (% pts gagnes serv.) |               |               |          |        |
| RPW (% pts gagnes ret.)  |               |               |          |        |
| Hold %                   |               |               |          |        |
| % BP sauves              |               |               |          |        |
| % BP convertis           |               |               |          |        |
| Aces/match               |               |               |          |        |
| DF/match                 |               |               |          |        |
| % 1ere balle in          |               |               |          |        |
| % pts gagnes 1ere balle  |               |               |          |        |
| % pts gagnes 2eme balle  |               |               |          |        |

Sources : [URL 1], [URL 2], ...

1.4 FORME RECENTE (10 derniers matchs)
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur A] results 2025" sur flashscore.com ou tennisexplorer.com
  2. Chercher "[Joueur B] results 2025" sur flashscore.com ou tennisexplorer.com
  3. Pour chaque match recent, noter le rang de l'adversaire au moment du match
  4. Fallback : chercher "[Joueur] recent matches" sur sofascore.com

Joueur A — 10 derniers matchs :
| # | Date | Tournoi | Surface | Adversaire (rang) | Score | Duree | W/L |
|---|------|---------|---------|-------------------|-------|-------|-----|
Bilan W-L : ___ | Sur cette surface : ___

Joueur B — 10 derniers matchs :
| # | Date | Tournoi | Surface | Adversaire (rang) | Score | Duree | W/L |
|---|------|---------|---------|-------------------|-------|-------|-----|
Bilan W-L : ___ | Sur cette surface : ___

Qualite des victoires (% adversaires top 50) : A = ___% | B = ___%

1.5 H2H COMPLET
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur A] vs [Joueur B] head to head" sur atptour.com (section H2H)
  2. Fallback : chercher "[Joueur A] vs [Joueur B] h2h" sur tennisexplorer.com
  3. Fallback : chercher "[Joueur A] [Joueur B] h2h tennis" sur Google

| # | Date | Tournoi | Surface | Score | Duree | Coeff recence | Source |
|---|------|---------|---------|-------|-------|---------------|--------|
| | | | | | | <12m=1.0 / 12-24=0.7 / 24-36=0.5 / >36=0.3 | |
Bonus meme surface que le match actuel : coeff x1.3
% victoires pondere A = ___

Bilan global H2H : A mene ___-___
Bilan sur cette surface : A mene ___-___
Pattern notable : [serveur dominant, tie-breaks, etc.]

1.6 SURFACE & HISTORIQUE TOURNOI
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur A] [surface] record 2024-25" sur tennisabstract.com (filtre surface)
  2. Chercher "[Joueur B] [surface] record 2024-25" sur tennisabstract.com
  3. Chercher "[Joueur A] [nom tournoi] results" sur atptour.com (historique tournoi)
  4. Chercher "[Joueur B] [nom tournoi] results" sur atptour.com

| Donnee                            | Joueur A      | Joueur B      | Source |
|-----------------------------------|---------------|---------------|--------|
| W-L sur cette surface (saison)    |               |               |        |
| W-L sur cette surface (carriere)  |               |               |        |
| W-L dans ce tournoi (carriere)    |               |               |        |
| Meilleur resultat dans ce tournoi |               |               |        |
| W-L au tour actuel (carriere)     |               |               |        |

1.7 BLESSURES & CONDITION PHYSIQUE
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur A] injury 2025" sur tennisexplorer.com ou Google actualites
  2. Chercher "[Joueur B] injury 2025" sur tennisexplorer.com ou Google actualites
  3. Chercher "tennis injuries list" sur rotowire.com/tennis/injury-report.php
  4. Verifier le calendrier recent : combien de matchs en 14 jours ?

| Donnee                    | Joueur A      | Joueur B      | Source |
|---------------------------|---------------|---------------|--------|
| Blessure recente (<3 mois)|               |               |        |
| MTO recents (medical timeout)|             |               |        |
| Matchs joues 14 derniers j|               |               |        |
| Duree totale matchs 7j    |               |               |        |
| Abandon/forfait recent    |               |               |        |

1.8 COTES & MOUVEMENT DE MARCHE
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur A] vs [Joueur B] odds" sur oddsportal.com
  2. Si introuvable : essayer "[Joueur A] [Joueur B] odds tennis" sur betexplorer.com
  3. Pour Challengers/ITF : les cotes peuvent etre absentes ou limitees a 2-3 books
     → noter [MARCHE ETROIT] si moins de 5 bookmakers disponibles
  4. Noter les cotes d'ouverture ET actuelles pour detecter le mouvement
  5. Comparer Pinnacle (sharp) vs ParionsSport pour detecter les ecarts

| Book        | Cote A | Cote B | Source |
|-------------|--------|--------|--------|
| ParionsSport|        |        | pari utilisateur |
| Pinnacle    |        |        | oddsportal |
| Betclic     |        |        | oddsportal |
| Winamax     |        |        | oddsportal |

Mouvement de ligne : ouverture ___ → actuel ___ (direction : ___)
Reverse line movement : ___

▶ STOP — Affiche TOUT le contenu ci-dessus avec TOUTES les valeurs remplies et TOUTES les sources listees AVANT de passer a la Phase 2.

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

⚠️ MALUS CONFIANCE pour tournois mineurs :
  - Challenger sans Elo : -2 pts sur "Qualite stats"
  - ITF / Qualifs : -3 pts sur "Qualite stats"
  - Stats calculees manuellement (<10 matchs) : -1 pt sur "Completude"
  - Marche etroit (<5 books) : -1 pt sur "Coherence consensus marche"

Kelly/4 = (P_finale * cote - 1) / (cote - 1) / 4 = ___% bankroll

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B] — [Tournoi] [Surface]         ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/20                    ║
║ FACTEURS CLES : 1. ___ 2. ___ 3. ___             ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
