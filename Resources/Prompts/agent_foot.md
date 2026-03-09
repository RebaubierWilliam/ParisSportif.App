═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DE DONNEES (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

⚠️ REGLE ABSOLUE : Recherche web OBLIGATOIRE pour CHAQUE section.
Tu dois effectuer AU MINIMUM 10 recherches web distinctes en Phase 1.
Chaque tableau doit contenir des VALEURS CONCRETES (chiffres, scores, dates).
Si introuvable apres 2 sources : [DONNEE MANQUANTE — Confiance reduite].
INTERDICTION d'inventer ou estimer une donnee sans source.

╔═══════════════════════════════════════════════════════════════╗
║ DETECTION NIVEAU DE LIGUE — Adapte ta strategie de recherche ║
╠═══════════════════════════════════════════════════════════════╣
║ TIER 1 (Top 5) : Ligue 1, Premier League, Liga, Serie A,    ║
║   Bundesliga → understat.com + fbref.com (xG disponible)     ║
║ TIER 2 (Second rang) : Championship, Serie B, Ligue 2,       ║
║   LaLiga2, 2.Bundesliga, Eredivisie, Liga Portugal,          ║
║   Super Lig → fbref.com + footystats.org (xG partiel)        ║
║ TIER 3 (Divisions inferieures / coupes nationales) :          ║
║   D3+, National, Primera Federacion, Serie C, etc.           ║
║   → soccerway.com + flashscore.com + sources LOCALES         ║
║   → xG souvent INDISPONIBLE → utiliser METHODE FORCE         ║
║ INTER-DIVISIONS (coupe, barrage) : equipes de niveaux        ║
║   differents → evaluer l'ecart de division comme facteur     ║
╚═══════════════════════════════════════════════════════════════╝

SOURCES LOCALES PAR PAYS (a utiliser en priorite pour TIER 2-3) :
  - Espagne : marca.com, as.com, resultadosfutbol.com, sport.es
  - France : lequipe.fr, footmercato.net, transfermarkt.fr
  - Italie : gazzetta.it, transfermarkt.it, tuttomercatoweb.com
  - Angleterre : bbc.com/sport/football, skysports.com
  - Allemagne : kicker.de, transfermarkt.de
  - Portugal : abola.pt, maisfutebol.iol.pt
  - Turquie : tff.org, mackolik.com
  - Pays-Bas : vi.nl, transfermarkt.nl
  - Belgique : walfoot.be, hln.be/sport
  → Chercher aussi les COMPTES OFFICIELS des clubs sur X/Twitter pour
    compos, convocations et infos blessures le jour du match.

MOTS-CLES DE RECHERCHE PAR LANGUE (pour ligues mineures) :
  - Espagnol : "[equipe] convocatoria", "[equipe] alineación probable",
    "[equipe] parte médico", "[equipe] lesión", "[equipe] últimas noticias"
  - Francais : "[equipe] compo probable", "[equipe] groupe",
    "[equipe] blessures", "[equipe] absences"
  - Italien : "[equipe] formazione probabile", "[equipe] infortunati"
  - Allemand : "[equipe] aufstellung", "[equipe] verletzte spieler"

1.1 CLASSEMENT & FORME
RECHERCHES A EFFECTUER (dans cet ordre, passer a la suivante si echec) :
  TIER 1-2 :
    1. Chercher "[Equipe A] classement [competition] 2024-2025" sur sofascore.com
    2. Chercher "[competition] standings 2024-25" sur fbref.com
  TIER 3 / LIGUES MINEURES :
    3. Chercher "[competition] classement" sur soccerway.com (couvre quasiment tout)
    4. Chercher "[competition] standings" sur flashscore.com
    5. Chercher "[competition] clasificación" (ou equivalent local) sur le site FA du pays
  DANS TOUS LES CAS :
    6. Pour Elo : chercher "[Equipe A]" et "[Equipe B]" sur clubelo.com
    7. Chercher "[Equipe A] results 2024-25" et "[Equipe B] results 2024-25" pour forme recente

| Donnee                    | Equipe A       | Equipe B       | Source |
|---------------------------|----------------|----------------|--------|
| Classement (Pos/Pts/+-)  |                |                |        |
| Forme 5 derniers (global) |                |                |        |
| Forme dom/ext (5 dern.)  |                |                |        |
| Elo (clubelo.com)         |                |                |        |

Si INTER-DIVISIONS : noter l'ecart de division et le rang relatif dans chaque ligue.
Sources : [lister chaque URL consultee]

1.2 xG & STATS AVANCEES
HIERARCHIE DE SOURCES — SUIVRE CET ORDRE STRICT SELON LE TIER :

  TIER 1 (Top 5 europeens) :
    1. understat.com → chercher "[Equipe] xG 2024-25"
    2. Fallback : fbref.com/en/squads/ → chercher "[Equipe] stats"

  TIER 2 (Secondes divisions, petites ligues europeennes) :
    1. fbref.com → chercher "[competition] stats" (verifier si la ligue est couverte)
    2. footystats.org → chercher "[Equipe] [competition] statistics"
    3. Fallback : flashscore.com → onglet "Statistiques" de l'equipe (tirs, possession, corners)
    4. Fallback : soccerway.com → stats de base (buts, resultats)

  TIER 3 (D3+, Nacional, Primera Federacion, etc.) :
    1. flashscore.com → onglet stats equipe (souvent le plus complet pour ces niveaux)
    2. soccerway.com → buts marques/encaisses par match
    3. sofascore.com → stats de base si disponible
    4. Source locale du pays (voir liste ci-dessus)
    → xG TRES PROBABLEMENT INDISPONIBLE → utiliser la METHODE FORCE (section 2.1)

RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] stats [saison]" sur la source prioritaire du tier
  2. Chercher "[Equipe B] stats [saison]" idem
  3. Chercher "[Equipe A] home stats" et "[Equipe B] away stats" pour dom/ext
  4. Si xG introuvable sur source 1, passer IMMEDIATEMENT a la suivante
  5. Si aucune source n'a de xG, collecter au minimum : buts/m, buts encaisses/m,
     tirs/m, tirs cadres/m, possession, corners — ces stats suffisent pour la methode force.

| Stat                | A (dom) | A (global) | B (ext) | B (global) | Source |
|---------------------|---------|------------|---------|------------|--------|
| Buts marques/m      |         |            |         |            |        |
| Buts encaisses/m    |         |            |         |            |        |
| xG pour/m           |         |            |         |            |        |
| xG contre/m         |         |            |         |            |        |
| xG L5 (5 derniers)  |         |            |         |            |        |
| Tirs/match          |         |            |         |            |        |
| Tirs cadres/m       |         |            |         |            |        |
| Possession %        |         |            |         |            |        |
| Corners/match       |         |            |         |            |        |
| BTTS %              |         |            |         |            |        |
| Over 2.5 %          |         |            |         |            |        |
| Over 1.5 %          |         |            |         |            |        |

⚠️ Si xG indisponible, remplir au minimum buts + tirs + tirs cadres.
   Mettre [N/A] pour les stats manquantes, PAS de cases vides.

Sources : [URL 1], [URL 2], ...

1.3 ABSENCES & COMPOSITION
RECHERCHES A EFFECTUER (toutes obligatoires) :
  POUR TOUTES LES LIGUES :
    1. Chercher "[Equipe A] injuries" sur transfermarkt.com
    2. Chercher "[Equipe B] injuries" sur transfermarkt.com
    3. Chercher "[Equipe A] vs [Equipe B] lineups" sur sofascore.com (onglet lineups)
  POUR LIGUES TIER 2-3 (transfermarkt souvent incomplet) :
    4. Chercher "[Equipe A] + [mot-cle local blessure]" sur Google actualites
       (ex: "UD Almería parte médico", "Cultural Leonesa convocatoria")
    5. Chercher "[Equipe B] + [mot-cle local blessure]" idem
    6. Chercher le compte X/Twitter officiel du club pour la convocatoria/groupe
    7. Source locale du pays (lequipe.fr, marca.com, gazzetta.it, kicker.de, etc.)
  COMPO PROBABLE :
    8. Chercher "[Equipe A] lineup [date]" sur Google actualites
    9. Chercher "[Equipe B] lineup [date]" sur Google actualites

| Joueur | Equipe | Poste | Raison (blessure/suspendu/incertain) | Titulaire ? | Impact xG | Source |
|--------|--------|-------|--------------------------------------|-------------|-----------|--------|

Regles impact xG :
Buteur principal absent : -0.15 a -0.30 xG equipe
DC/DG titulaire absent : +0.10 a +0.20 xG adverse
GK titulaire absent : +0.10 a +0.15 xG adverse
Milieu createur (meneur jeu) absent : -0.10 a -0.20 xG equipe
Arriere lateral offensif absent : -0.05 a -0.10 xG equipe

1.4 H2H (5 derniers)
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] vs [Equipe B] head to head" sur sofascore.com
  2. Si insuffisant : chercher "[Equipe A] vs [Equipe B] h2h" sur flashscore.com
  3. Chercher "[Equipe A] vs [Equipe B] historique" sur soccerway.com
  4. Fallback Google : "[Equipe A] vs [Equipe B] resultats historiques"

  SI PEU OU PAS DE H2H DIRECTS (equipes de divisions differentes, etc.) :
    5. Signaler [H2H INSUFFISANT — X matchs trouves]
    6. Compenser en analysant les performances contre des adversaires de niveau
       similaire. Ex: comment A joue contre les equipes du niveau de B dans sa ligue.
    7. Reduire le poids du critere H2H en Phase 2 (→ redistribuer sur Forme et Stats).

| Date | Competition | Score | Lieu (dom/ext) | Coeff recence | Source |
|------|-------------|-------|----------------|---------------|--------|
| | | | | <6m=1.0 / 6-12=0.8 / 12-24=0.6 / >24=0.4 | |

Tendance H2H : [decrire pattern dominant, ou "H2H insuffisant"]

1.5 CONTEXTE [-2 a +2 par facteur]
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] calendrier [mois]" pour evaluer fatigue/enjeu
  2. Chercher "[Equipe B] calendrier [mois]" idem
  3. Chercher "[Equipe A] actualite" et "[Equipe B] actualite" pour motivation/contexte
  4. Si INTER-DIVISIONS : evaluer la motivation du petit club (effet coupe)
     et le risque de sous-estimation du grand club

| Facteur            | A   | B   | Justification (avec source) |
|--------------------|-----|-----|----------------------------|
| Enjeu              |     |     |                            |
| Fatigue/calendrier |     |     |                            |
| Momentum           |     |     |                            |
| Motivation         |     |     |                            |
| Ecart de division  |     |     | (si applicable)            |
TOTAL CONTEXTE : A = ___ / B = ___

1.6 MOYENNES LIGUE
RECHERCHES A EFFECTUER :
  TIER 1 :
    1. Chercher "[competition] stats 2024-25" sur understat.com (page ligue)
  TIER 2 :
    2. Chercher "[competition] statistics" sur fbref.com
    3. Fallback : chercher "[competition] stats" sur footystats.org
  TIER 3 / LIGUES MINEURES :
    4. Chercher "[competition] statistics" sur flashscore.com (rubrique "Statistiques" de la ligue)
    5. Chercher "[competition] stats" sur soccerway.com
    6. CALCUL MANUEL si aucune source directe : recuperer le total de buts marques
       et le nombre de matchs joues depuis le classement, puis diviser.
       Formule : Moy = Total_buts_ligue / Total_matchs_joues

  SI INTER-DIVISIONS : collecter les moyennes des DEUX ligues separement.

Buts/match global : ___ | Dom : ___ | Ext : ___ | Source : ___
(Si inter-divisions : Ligue A = ___ | Ligue B = ___)

1.7 COTES & MOUVEMENT DE MARCHE
RECHERCHES A EFFECTUER :
  1. Chercher "[Equipe A] vs [Equipe B] odds" sur oddsportal.com
  2. Si introuvable : essayer "[Equipe A] [Equipe B] odds" sur betexplorer.com
  3. Essayer aussi des variantes de noms (nom complet, abreviation, nom local)
     Ex: "Almeria Cultural Leonesa odds" ou "UD Almeria Cultural y Deportiva"
  4. Noter le mouvement de ligne (ouverture → actuel) si visible

| Book        | Cote 1 | Cote N | Cote 2 | Source |
|-------------|--------|--------|--------|--------|
| ParionsSport|        |        |        | pari utilisateur |
| Pinnacle    |        |        |        | oddsportal |
| Betclic     |        |        |        | oddsportal |
| Winamax     |        |        |        | oddsportal |

Mouvement de ligne : ouverture ___ → actuel ___ (direction : ___)
Pinnacle = reference sharp. Si ecart > 10 centimes avec ParionsSport → signal.

▶ STOP — Affiche TOUT le contenu ci-dessus avec TOUTES les valeurs remplies et TOUTES les sources listees AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ANALYSE MATHEMATIQUE (tous les calculs visibles)
═══════════════════════════════════════════════════════

2.1 LAMBDA (buts attendus)

METHODE A — xG DISPONIBLE (Tier 1, Tier 2 partiel) :
  lambda_A = (xG_A_dom + xG_contre_B_ext) / 2
  lambda_B = (xG_B_ext + xG_contre_A_dom) / 2
  Si coupe UEFA (<4 matchs en coupe) : lambda = 0.6*lambda_ligue + 0.4*lambda_coupe

METHODE B — FORCE CLASSIQUE (quand xG indisponible) :
  ⚠️ SIGNALER : [METHODE FORCE — xG indisponible pour cette ligue]
  1. Calculer les forces relatives :
     Force_att_A = Buts_marques_A_dom / Moy_buts_dom_ligue
     Force_def_B = Buts_encaisses_B_ext / Moy_buts_ext_ligue
     (idem pour B attaque et A defense)
  2. Calculer les lambdas :
     lambda_A = Force_att_A * Force_def_B * Moy_buts_dom_ligue
     lambda_B = Force_att_B * Force_def_A * Moy_buts_ext_ligue
  3. Si les tirs cadres sont disponibles, croiser avec le ratio
     tirs cadres / buts pour valider la coherence.

METHODE C — INTER-DIVISIONS (equipes de ligues differentes) :
  ⚠️ SIGNALER : [INTER-DIVISIONS — estimation avec incertitude elevee]
  1. Calculer lambda dans chaque ligue respective (methode A ou B)
  2. Appliquer un coefficient d'ecart de division :
     - 1 division d'ecart : equipe superieure x1.15-1.25, inferieure x0.75-0.85
     - 2 divisions d'ecart : equipe superieure x1.30-1.50, inferieure x0.60-0.70
  3. Ponderer par l'avantage du terrain et le contexte (coupe = motivation elevee petit club)

Ajustements (communs aux 3 methodes) :
| Source            | Regle                          | Valeur |
|-------------------|--------------------------------|--------|
| Absences (1.3)    | Somme impacts xG               | +/-    |
| Momentum >=4V     | x1.05 a x1.10                 | x      |
| Fatigue >=3m/10j  | x0.90 a x0.95                 | x      |
| Contexte (1.5)    | +/-0.03 par pt net d'ecart     | +/-    |
| Derby/finale      | Resserrement lambdas ~5%        | +/-    |

lambda_A ajuste = ___ | lambda_B ajuste = ___
Methode utilisee : [A / B / C]

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

Marches derives :
| Marche       | P.simulee | Source calcul |
|--------------|-----------|---------------|
| BTTS Oui     |     %     | Matrice       |
| BTTS Non     |     %     | Matrice       |
| Over 2.5     |     %     | Matrice       |
| Under 2.5    |     %     | Matrice       |
| Over 1.5     |     %     | Matrice       |

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

⚠️ MALUS CONFIANCE pour ligues mineures :
  - Tier 2 sans xG : -2 pts automatique sur "Qualite donnees"
  - Tier 3 : -3 pts automatique sur "Qualite donnees"
  - Inter-divisions : -2 pts automatique sur "Stabilite lambda"
  - H2H insuffisant (<3 matchs) : -2 pts sur "Coherence H2H"

╔═══════════════════════════════════════════════════╗
║ MATCH : [A] vs [B]                                ║
║ RECOMMANDATION : [Jouer X @ cote Y / Ne pas jouer]║
║ P.simulee : X% | P.marche : X% | Ecart : +X%    ║
║ EV : +X.XX | Confiance : __/25                    ║
║ FACTEURS CLES : 1. ___ 2. ___ 3. ___             ║
║ RISQUES : 1. ___ 2. ___                           ║
╚═══════════════════════════════════════════════════╝
