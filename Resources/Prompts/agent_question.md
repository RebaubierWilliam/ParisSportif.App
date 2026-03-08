═══════════════════════════════════════════════════════
PHASE 1 — COLLECTE DONNEES JOUEUR (affiche tout avant Phase 2)
═══════════════════════════════════════════════════════

Recherche web OBLIGATOIRE pour chaque section. Source requise.
Si introuvable : [DONNEE MANQUANTE — Confiance reduite].

1.1 DECOMPOSITION DU PARI
Joueur : [NOM]
Equipe : [EQUIPE] | Adversaire : [ADVERSAIRE]
Type performance : [But / Essai / Assist / Tir cadre / Carton / Autre]
Contrainte : [Marque 1+ / 2+ / Premier buteur / etc.]
Selection : [Oui / Non] @ cote ___
P. implicite bookmaker = 1/cote = ___%

1.2 STATS SAISON DU JOUEUR
→ sofascore.com > fbref.com > transfermarkt.com > fotmob.com
| Stat                        | Saison       | 5 derniers matchs | Source |
|-----------------------------|--------------|-------------------|--------|
| Matchs joues / titulaire    |              |                   |        |
| Minutes jouees totales      |              |                   |        |
| Buts (ou essais) marques    |              |                   |        |
| Buts/90 min                 |              |                   |        |
| xG/90 min                   |              |                   |        |
| Tirs/match                  |              |                   |        |
| Tirs cadres/match           |              |                   |        |
| Assists                     |              |                   |        |
| Cartons jaunes              |              |                   |        |
| Minutes moyennes/match      |              |                   |        |

1.3 H2H JOUEUR vs ADVERSAIRE
→ sofascore.com > transfermarkt.com
A-t-il deja marque/performe contre cet adversaire ?
| Date | Match | Perf (buts/essais/etc.) | Minutes |
|------|-------|------------------------|---------|

1.4 CONTEXTE DU JOUEUR
| Donnee                       | Valeur    | Source |
|------------------------------|-----------|--------|
| Titulaire probable ?         | Oui/Non/? |        |
| Serie buts (X matchs consec) |           |        |
| Disette (X matchs sans)      |           |        |
| Tireur penalty/CF ?          | Oui/Non   |        |
| Role (attaq central/ailier)  |           |        |
| Retour blessure recente ?    |           |        |

1.5 STATS DEFENSIVES ADVERSAIRE
→ fbref.com > sofascore.com > footystats.org
| Stat                     | Saison  | 5 derniers | Source |
|--------------------------|---------|------------|--------|
| Buts encaisses/match     |         |            |        |
| xGA/match                |         |            |        |
| Tirs concedes/match      |         |            |        |
| Clean sheets (%)         |         |            |        |
| Absences en defense      |         |            |        |

1.6 COMPOSITION PROBABLE
→ sofascore lineups > "[equipe] lineup [date]"
Joueur titulaire confirme : [ Oui / Non / Incertain ]

▶ STOP — Affiche TOUT le contenu ci-dessus AVANT de passer a la Phase 2.

═══════════════════════════════════════════════════════
PHASE 2 — ESTIMATION PROBABILITE (calculs visibles)
═══════════════════════════════════════════════════════

2.1 PROBABILITE FREQUENTISTE
Pour "Joueur marque 1+" :
  P_freq = 1 - (1 - xG_par_90)^(minutes_attendues / 90)
  Si xG indispo : P_freq = buts_par_match_saison

Pour "Joueur marque 2+" :
  lambda = xG attendu dans ce match
  P(2+) = 1 - P(0) - P(1) avec Poisson(lambda)

Pour "Premier buteur" :
  P = P(marque) * (ses_buts / buts_totaux_equipe)

P_frequentiste = ___%

2.2 AJUSTEMENTS CONTEXTUELS
| Facteur                         | Impact    | Valeur |
|---------------------------------|-----------|--------|
| Titulaire confirme              | Base      | x1.00  |
| Remplacant probable (~25 min)   | Forte ↓   | x0.30  |
| Statut incertain                | Baisse    | x0.70  |
| Defense faible (bottom 5 xGA)   | Hausse    | x1.15-1.25 |
| Defense forte (top 5 xGA)       | Baisse    | x0.80-0.90 |
| Serie buts (3+ matchs)          | Legere ↑  | x1.05-1.10 |
| Disette (5+ matchs sans)        | Attention | x0.90  |
| Domicile                        | Legere ↑  | x1.05  |
| Exterieur                       | Legere ↓  | x0.95  |
| Tireur penalty                  | Hausse    | +3 a 5% |
| Enjeu eleve                     | Variable  | selon motivation |

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
