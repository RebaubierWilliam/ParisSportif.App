═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DONNEES JOUEUR (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

⚠️ REGLE ABSOLUE : Recherche web OBLIGATOIRE pour CHAQUE section.
Tu dois effectuer AU MINIMUM 8 recherches web distinctes en Phase 1.
Chaque tableau doit contenir des VALEURS CONCRETES (chiffres, scores, dates).
Si introuvable apres 2 sources : [DONNEE MANQUANTE — Confiance reduite].
INTERDICTION d'inventer ou estimer une donnee sans source.

╔═══════════════════════════════════════════════════════════════╗
║ JOUEUR DE LIGUE MINEURE — Strategie adaptee                   ║
╠═══════════════════════════════════════════════════════════════╣
║ Pour les joueurs de D2/D3/ligues mineures, les stats          ║
║ individuelles (xG, tirs cadres) sont souvent INDISPONIBLES.   ║
║ Dans ce cas :                                                  ║
║ 1. Chercher le profil joueur sur sofascore.com ou flashscore  ║
║ 2. Chercher sur transfermarkt.com (buts/assists par saison)   ║
║ 3. Utiliser les stats par match sur flashscore.com/sofascore  ║
║ 4. Calculer manuellement buts/match a partir de l'historique  ║
║ 5. Chercher en LANGUE LOCALE : "[joueur] goles/buts/tore"    ║
║ 6. Signaler [STATS LIMITEES — echantillon X matchs]           ║
╚═══════════════════════════════════════════════════════════════╝

1.1 DECOMPOSITION DU PARI
Joueur : [NOM]
Equipe : [EQUIPE] | Adversaire : [ADVERSAIRE]
Type performance : [But / Essai / Assist / Tir cadre / Carton / Points / Rebonds / Autre]
Contrainte : [Marque 1+ / 2+ / Premier buteur / Over X.5 / etc.]
Selection : [Oui / Non] @ cote ___
P. implicite bookmaker = 1/cote = ___%

1.2 STATS SAISON DU JOUEUR
RECHERCHES A EFFECTUER (dans cet ordre de priorite) :
  FOOTBALL — TOP LIGUES (Ligue 1, PL, Liga, Serie A, Bundesliga) :
    1. Chercher "[Joueur] stats 2024-25" sur sofascore.com (profil joueur)
    2. Chercher "[Joueur] stats" sur fbref.com/en/players/ (stats detaillees + xG)
    3. Fallback : chercher "[Joueur] statistics [saison]" sur transfermarkt.com
    4. Fallback : chercher "[Joueur] stats" sur fotmob.com
  FOOTBALL — LIGUES MINEURES (D2, D3, etc.) :
    1. Chercher "[Joueur] profil" sur sofascore.com (souvent disponible meme en D2)
    2. Chercher "[Joueur]" sur transfermarkt.com (buts/assists toujours disponibles)
    3. Chercher "[Joueur]" sur flashscore.com → profil joueur → stats par match
    4. Chercher "[Joueur] [equipe] goles/buts" en langue locale sur Google actualites
    5. xG INDISPONIBLE en D2/D3 → calculer buts/match manuellement
       signaler [xG INDISPO — utilisation buts/match sur X matchs]
  RUGBY :
    1. Chercher "[Joueur] stats [competition]" sur espn.com/rugby
    2. Fallback : chercher "[Joueur] rugby stats" sur rugbypass.com
    3. Fallback : chercher "[Joueur] profile" sur ultimaterugby.com
  BASKETBALL — NBA :
    1. Chercher "[Joueur] stats 2024-25" sur basketball-reference.com/players/
    2. Chercher "[Joueur] game log" sur nba.com/stats/player/
    3. Pour props : chercher "[Joueur] props" sur statmuse.com
  BASKETBALL — LIGUES EUROPEENNES :
    1. Chercher "[Joueur] stats" sur flashscore.com ou sofascore.com
    2. Chercher sur le site officiel de la ligue (euroleaguebasketball.net, acb.com, etc.)
  TENNIS :
    1. Chercher "[Joueur] stats" sur tennisabstract.com
    2. Fallback (Challengers/ITF) : flashscore.com + matchstat.com
  POUR TOUS LES SPORTS :
    6. Chercher "[Joueur] derniers matchs [saison]" pour la forme L5

| Stat                        | Saison       | 5 derniers matchs | Source |
|-----------------------------|--------------|-------------------|--------|
| Matchs joues / titulaire    |              |                   |        |
| Minutes jouees totales      |              |                   |        |
| Buts (ou perf visee) total  |              |                   |        |
| Buts/90 min (ou perf/match) |              |                   |        |
| xG/90 min (si foot)         |              |                   |        |
| npxG/90 min (si foot)       |              |                   |        |
| Tirs/match                  |              |                   |        |
| Tirs cadres/match           |              |                   |        |
| Assists                     |              |                   |        |
| Cartons jaunes              |              |                   |        |
| Minutes moyennes/match      |              |                   |        |

Detail des 5 derniers matchs :
| # | Date | Adversaire | Score | Perf (buts/pts/etc) | Minutes | Titulaire? | Source |
|---|------|-----------|-------|---------------------|---------|------------|--------|

1.3 H2H JOUEUR vs ADVERSAIRE
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur] vs [Equipe adverse] stats" sur sofascore.com
  2. Chercher "[Joueur] vs [Equipe adverse]" sur transfermarkt.com (foot) ou basketball-reference (NBA)
  3. Chercher "[Joueur] goals vs [Equipe adverse]" sur Google

A-t-il deja marque/performe contre cet adversaire ?
| # | Date | Match | Perf (buts/essais/pts) | Minutes | Source |
|---|------|-------|-----------------------|---------|--------|

Bilan historique vs cet adversaire : ___ perf en ___ matchs

1.4 CONTEXTE DU JOUEUR
RECHERCHES A EFFECTUER :
  1. Chercher "[Joueur] news [date]" sur Google actualites (blessure, forme, declarations)
  2. Chercher "[Equipe] lineup [date match]" sur sofascore.com (compo probable)
  3. Chercher "[Equipe] compo probable" sur Google actualites ou lequipe.fr (si foot FR)
  4. Chercher "[Joueur] penalty taker" ou "[Joueur] set pieces" (tireur attitré ?)
  5. Chercher "[Joueur] injury" pour verifier condition physique

| Donnee                       | Valeur    | Source |
|------------------------------|-----------|--------|
| Titulaire probable ?         | Oui/Non/? |        |
| Serie performances (X matchs consec) |   |        |
| Disette (X matchs sans)      |           |        |
| Tireur penalty/CF/lancers ?  | Oui/Non   |        |
| Role tactique precis         |           |        |
| Retour blessure recente ?    |           |        |
| Temps de jeu L5 (moyenne min)|           |        |

1.5 STATS DEFENSIVES ADVERSAIRE
RECHERCHES A EFFECTUER :
  FOOTBALL — TOP LIGUES :
    1. Chercher "[Equipe adverse] defensive stats" sur fbref.com/en/squads/
    2. Chercher "[Equipe adverse] xGA 2024-25" sur fbref.com ou understat.com
    3. Chercher "[Equipe adverse] goals conceded" sur sofascore.com
    4. Fallback : chercher "[Equipe adverse] statistics" sur footystats.org
    5. Chercher "[Equipe adverse] injuries defenders" sur transfermarkt.com
  FOOTBALL — LIGUES MINEURES (xGA indisponible) :
    1. Chercher "[Equipe adverse] stats" sur flashscore.com → buts encaisses/match
    2. Chercher "[Equipe adverse] classement" sur soccerway.com → diff de buts
    3. Chercher "[Equipe adverse] results" sur sofascore.com → compter clean sheets
    4. Signaler [xGA INDISPO — utilisation buts encaisses/match]
  BASKETBALL :
    1. Chercher "[Equipe adverse] defensive stats" sur nba.com/stats/teams/defense
    2. Chercher "[Equipe adverse] opponent stats" sur basketball-reference.com
  RUGBY :
    1. Chercher "[Equipe adverse] defence stats" sur espn.com/rugby
  POUR TOUS :
    6. Chercher "[Equipe adverse] derniers matchs buts encaisses" pour forme def recente

| Stat                     | Saison  | 5 derniers | Rang relatif | Source |
|--------------------------|---------|------------|--------------|--------|
| Buts/pts encaisses/match |         |            |              |        |
| xGA/match (si foot)      |         |            |              |        |
| Tirs concedes/match      |         |            |              |        |
| Clean sheets (%)         |         |            |              |        |
| Absences en defense      |         |            |              |        |

Classification defense adverse : [elite / solide / moyenne / faible / passoire]

1.6 COMPOSITION PROBABLE
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe] vs [Adversaire] predicted lineup" sur sofascore.com
  2. Chercher "[Equipe] lineup [date]" sur Google actualites
  3. Si foot FR : chercher "[Equipe] compo" sur lequipe.fr
  4. Si NBA : chercher "[Equipe] starting lineup" sur rotowire.com

Joueur titulaire confirme : [ Oui / Non / Incertain ] | Source : ___
Si incertain : P(titulaire) estimee = ___% (basee sur les L5 matchs)
Position prevue dans le schema tactique : ___

▶ STOP — Affiche TOUT le contenu ci-dessus avec TOUTES les valeurs remplies et TOUTES les sources listees AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ESTIMATION PROBABILITE (calculs visibles)
═══════════════════════════════════════════════════════

2.1 PROBABILITE FREQUENTISTE
Pour "Joueur marque 1+" (ou perf 1+) :
  P_freq = 1 - (1 - xG_par_90)^(minutes_attendues / 90)
  Si xG indispo : P_freq = perf_par_match_saison (taille echantillon = ___ matchs)

Pour "Joueur marque 2+" :
  lambda = xG attendu dans ce match
  P(2+) = 1 - P(0) - P(1) avec Poisson(lambda)

Pour "Premier buteur" :
  P = P(marque) * (ses_buts / buts_totaux_equipe)

Pour "Over X.5" (NBA/basket props) :
  P = % de matchs ou le joueur a depasse X sur la saison

P_frequentiste = ___%

2.2 AJUSTEMENTS CONTEXTUELS
| Facteur                         | Impact    | Valeur | Justification |
|---------------------------------|-----------|--------|---------------|
| Titulaire confirme              | Base      | x1.00  |               |
| Remplacant probable (~25 min)   | Forte ↓   | x0.30  |               |
| Statut incertain                | Baisse    | x0.70  |               |
| Defense faible (bottom 5 xGA)   | Hausse    | x1.15-1.25 |           |
| Defense forte (top 5 xGA)       | Baisse    | x0.80-0.90 |           |
| Serie performances (3+ matchs)  | Legere ↑  | x1.05-1.10 |           |
| Disette (5+ matchs sans)        | Attention | x0.90  |               |
| Domicile                        | Legere ↑  | x1.05  |               |
| Exterieur                       | Legere ↓  | x0.95  |               |
| Tireur penalty/lancers francs   | Hausse    | +3 a 5% |              |
| Defenseur adverse absent        | Hausse    | x1.05-1.15 |           |
| Enjeu eleve                     | Variable  | selon motivation |      |

P_ajustee = P_freq * produit(facteurs) = ___%

2.3 EV & COMPARAISON MARCHE
| Sel  | Cote  | P.impl (1/cote) | P.simulee | Ecart  | EV              |
|------|-------|-----------------|-----------|--------|-----------------|
| Oui  |       |       %         |     %     | +/- %  | (P_sim*cote)-1  |
| Non  |       |       %         |     %     | +/- %  | (P_sim*cote)-1  |

2.4 VERDICT
| Ecart (Sim - Marche) | EV        | Verdict                  |
|----------------------|-----------|--------------------------|
| > +8%                | > +0.10   | VALUE BET FORTE ***      |
| +4% a +8%            | +0.05/10  | VALUE BET CONFIRMEE **   |
| +2% a +4%            | +0.02/05  | VALUE BET LEGERE *       |
| 0 a +2%              | 0 a +0.02 | ZONE NEUTRE              |
| < 0                  | < 0       | PAS DE VALUE             |

Seuils plus bas que 1N2 (precision statistique moindre sur les props).

Confiance (/20) :
| Critere                                  | Score |
|------------------------------------------|-------|
| Qualite stats joueur (xG dispo, taille)  | /5    |
| Certitude titularisation                  | /5    |
| Fiabilite defense adverse (echantillon)   | /5    |
| Coherence tendance recente                | /5    |
TOTAL : ___/20 (16-20=elevee, 12-15=moderee, <12=faible)

⚠️ MALUS CONFIANCE pour joueurs de ligues mineures :
  - xG indisponible (buts/match utilise) : -2 pts sur "Qualite stats joueur"
  - Echantillon < 10 matchs : -1 pt sur "Qualite stats joueur"
  - Defense adverse sans xGA : -1 pt sur "Fiabilite defense adverse"

╔═══════════════════════════════════════════════════╗
║ QUESTION : [Enonce complet]                       ║
║ JOUEUR : [Nom] ([Equipe])                         ║
║ SELECTION : [Oui/Non] @ cote X.XX                ║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/20                    ║
║ RECOMMANDATION : [VALUE BET / NE PAS JOUER]       ║
║ FACTEURS CLES : 1. ___ 2. ___                    ║
║ RISQUES : 1. ___ (non-titularisation?) 2. ___    ║
╚═══════════════════════════════════════════════════╝
