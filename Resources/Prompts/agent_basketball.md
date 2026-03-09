═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

⚠️ REGLE ABSOLUE : Recherche web OBLIGATOIRE pour CHAQUE section.
Tu dois effectuer AU MINIMUM 10 recherches web distinctes en Phase 1.
Chaque tableau doit contenir des VALEURS CONCRETES (chiffres, scores, dates).
Si introuvable apres 2 sources : [DONNEE MANQUANTE — Confiance reduite].
INTERDICTION d'inventer ou estimer une donnee sans source.

╔═══════════════════════════════════════════════════════════════╗
║ DETECTION NIVEAU DE LIGUE — Adapte ta strategie              ║
╠═══════════════════════════════════════════════════════════════╣
║ TIER 1 : NBA → nba.com/stats + basketball-reference.com      ║
║ TIER 2 : Euroleague, Eurocup, ACB, LNB Pro A, BBL            ║
║   → euroleaguebasketball.net + flashscore.com + sofascore.com ║
║ TIER 3 : Pro B, LEB Oro, Serie A2, ligues mineures            ║
║   → flashscore.com + sofascore.com + sites locaux             ║
║   → Stats avancees (ORTG/DRTG) souvent INDISPONIBLES          ║
║   → Utiliser buts/pts marques/encaisses comme proxy            ║
╚═══════════════════════════════════════════════════════════════╝

SOURCES PAR LIGUE EUROPEENNE :
  - Euroleague/Eurocup : euroleaguebasketball.net (stats completes)
  - Espagne ACB : acb.com, marca.com/baloncesto
  - France LNB : lnb.fr, basketeurope.com
  - Allemagne BBL : easycredit-bbl.de
  - Italie Lega : legabasket.it
  - Turquie BSL : basketbol.tbl.org.tr
  - Grece : esake.gr
  → flashscore.com et sofascore.com couvrent la plupart de ces ligues

1.1 CONTEXTE DU MATCH
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] vs [Equipe B] [date]" sur nba.com ou espn.com/nba/schedule
  2. Chercher "[Equipe A] schedule" pour detecter back-to-back
  3. Chercher "[Equipe B] schedule" idem
  4. Si Euroleague/Eurocup : chercher sur euroleaguebasketball.net

Enjeu : [playoffs, play-in, tanking, saison reguliere, elimination]
Phase saison : [pre All-Star, post All-Star, playoffs]
Back-to-back : A → [O/N, si oui quel match la veille] | B → [O/N]
Repos avant ce match : A = ___ jours | B = ___ jours

1.2 BLESSURES & ABSENCES — FACTEUR CRITIQUE N°1
RECHERCHES A EFFECTUER (TOUTES obligatoires, c'est le facteur le plus important) :
  1. Chercher "[Equipe A] injury report" sur espn.com/nba/team/injuries
  2. Chercher "[Equipe B] injury report" sur espn.com/nba/team/injuries
  3. Chercher "[Equipe A] injury report" sur rotowire.com/basketball/nba-injury-report.php
  4. Chercher "[Equipe B] injury report" sur rotowire.com/basketball/nba-injury-report.php
  5. Cross-checker avec cbssports.com/nba/injuries/
  6. Chercher "[Equipe A] [Equipe B] injury news [date]" sur Google actualites
  7. Si Euroleague : chercher sur euroleaguebasketball.net/euroleague/news/

Equipe A — Absences :
| Joueur | Statut (Out/Doubtful/Quest) | Role (Star/Tit/Rot) | PPG | BPM | On/Off diff | Source |
|--------|-----------------------------|---------------------|-----|-----|-------------|--------|

Equipe B — Absences :
| Joueur | Statut (Out/Doubtful/Quest) | Role (Star/Tit/Rot) | PPG | BPM | On/Off diff | Source |
|--------|-----------------------------|---------------------|-----|-----|-------------|--------|

Regles d'impact :
Star absent (top 1-2 du roster) : malus -3 a -8 pts sur note globale selon BPM
Titulaire absent : malus -1 a -3 pts
Mouvement de ligne >10 centimes = possible info blessure non publique → SIGNALER

1.3 STATS AVANCEES (saison)
RECHERCHES A EFFECTUER :
  TIER 1 (NBA) :
    1. Chercher "[Equipe A] stats 2024-25" sur nba.com/stats/teams/advanced
    2. Chercher "[Equipe B] stats 2024-25" idem
    3. Chercher "[Equipe A] team stats" sur basketball-reference.com/teams/
    4. Chercher "[Equipe B] team stats" idem
    5. Pour les "Four Factors" : basketball-reference.com → Team Stats → Advanced
    6. Chercher "[Equipe A] home stats" et "[Equipe B] away stats" pour splits dom/ext
  TIER 2 (Euroleague, ACB, LNB, etc.) :
    1. Chercher sur le site officiel de la ligue (euroleaguebasketball.net, acb.com, lnb.fr)
    2. Chercher "[Equipe] stats" sur flashscore.com → onglet statistiques
    3. Chercher "[Equipe] stats" sur sofascore.com
    4. Si ORTG/DRTG indisponibles : utiliser pts marques/match et pts encaisses/match
  TIER 3 (Pro B, LEB Oro, ligues mineures) :
    1. flashscore.com → onglet stats equipe (souvent la meilleure source)
    2. sofascore.com → stats de base
    3. Site officiel de la ligue si existant
    → ORTG/DRTG/Four Factors tres probablement INDISPONIBLES
    → Se concentrer sur : pts/match, pts encaisses/match, W-L dom/ext, forme

| Metrique          | Equipe A | Rang | Equipe B | Rang | Source |
|-------------------|----------|------|----------|------|--------|
| ORTG              |          |      |          |      |        |
| DRTG              |          |      |          |      |        |
| Net Rating        |          |      |          |      |        |
| Pace              |          |      |          |      |        |
| eFG%              |          |      |          |      |        |
| TOV%              |          |      |          |      |        |
| OREB%             |          |      |          |      |        |
| FT Rate           |          |      |          |      |        |
| Points in Paint   |          |      |          |      |        |
| 3PT%              |          |      |          |      |        |
| Opp 3PT%          |          |      |          |      |        |
| Fast Break Pts    |          |      |          |      |        |

Les "Four Factors" (eFG%, TOV%, OREB%, FT Rate) = meilleurs predicteurs.

1.4 FORME RECENTE (10 derniers matchs)
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] last 10 games" sur basketball-reference.com ou nba.com/stats
  2. Chercher "[Equipe B] last 10 games" idem
  3. Chercher "[Equipe A] game log 2024-25" sur basketball-reference.com
  4. Chercher "[Equipe B] game log 2024-25" sur basketball-reference.com
  5. Pour la qualite des adversaires : croiser les W-L avec les rangs adverses

| Donnee                    | Equipe A      | Equipe B      | Source |
|---------------------------|---------------|---------------|--------|
| Bilan W-L (total)         |               |               |        |
| Bilan dom/ext (L10)       |               |               |        |
| Net Rating L10            |               |               |        |
| ORTG L10                  |               |               |        |
| DRTG L10                  |               |               |        |
| Qualite adversaires L10   |               |               |        |
| Tendance (↑ → ↓)         |               |               |        |
| Scores des 5 derniers     |               |               |        |

1.5 JOUEURS CLES (3-4 par equipe)
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] roster stats 2024-25" sur basketball-reference.com
  2. Chercher "[Equipe B] roster stats 2024-25" sur basketball-reference.com
  3. Pour forme recente L5 : game logs des joueurs cles

Equipe A :
| Joueur | PPG | RPG | APG | TS% | BPM | Forme L5 (PPG) | Source |
|--------|-----|-----|-----|-----|-----|----------------|--------|

Equipe B :
| Joueur | PPG | RPG | APG | TS% | BPM | Forme L5 (PPG) | Source |
|--------|-----|-----|-----|-----|-----|----------------|--------|

1.6 H2H (5 derniers)
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] vs [Equipe B] head to head" sur basketball-reference.com
  2. Fallback : chercher "[Equipe A] vs [Equipe B] h2h" sur sofascore.com ou flashscore.com
  3. Chercher "[Equipe A] vs [Equipe B] season series 2024-25" pour la saison en cours

| # | Date | Lieu | Score | Absences notables | Top scoreur | Coeff recence | Source |
|---|------|------|-------|-------------------|-------------|---------------|--------|
| | | | | | | <6m=1.0 / 6-12=0.7 / 12-18=0.5 / >18=0.3 | |

Pattern H2H : [equipe dominante, ecarts, etc.]

1.7 FATIGUE & CALENDRIER
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] schedule [mois]" sur nba.com ou espn.com
  2. Chercher "[Equipe B] schedule [mois]" idem
  3. Verifier les fuseaux horaires et distances de deplacement si pertinent

| Facteur                    | Equipe A | Equipe B | Source |
|----------------------------|----------|----------|--------|
| Matchs en 7 jours         |          |          |        |
| Back-to-back               |          |          |        |
| Repos avant ce match (j)  |          |          |        |
| Distance parcourue (7j)   |          |          |        |
| Matchs OT recents         |          |          |        |

B2B = -1.5 a -2 pts de Net Rating en moyenne.
3 matchs en 4 jours = fatigue significative.

1.8 COTES & MOUVEMENT DE MARCHE
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] vs [Equipe B] odds" sur oddsportal.com
  2. Chercher "[Equipe A] vs [Equipe B] odds NBA" sur Google pour alternatives
  3. Noter mouvement d'ouverture → actuel

| Book        | Cote A | Cote B | Source |
|-------------|--------|--------|--------|
| ParionsSport|        |        | pari utilisateur |
| Pinnacle    |        |        | oddsportal |
| Betclic     |        |        | oddsportal |
| Winamax     |        |        | oddsportal |

Mouvement de ligne : ouverture ___ → actuel ___ (direction : ___)
Reverse line movement (public vs sharp) : ___
Si mouvement > 10 centimes → chercher la cause (blessure, load management, etc.)

▶ STOP — Affiche TOUT le contenu ci-dessus avec TOUTES les valeurs remplies et TOUTES les sources listees AVANT de passer a la Phase 2.

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

⚠️ MALUS CONFIANCE pour ligues mineures :
  - Tier 2 sans ORTG/DRTG : -1 pt sur "Qualite donnees"
  - Tier 3 : -2 pts sur "Qualite donnees"
  - Blessures non confirmees (source unique) : -1 pt sur "Completude"

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B]                                ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/20                    ║
║ FACTEURS CLES : 1. ___ 2. ___ 3. ___             ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
