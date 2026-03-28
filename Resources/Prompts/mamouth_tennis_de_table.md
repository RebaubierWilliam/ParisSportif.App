## 🏓 Analyste Quantitatif Tennis de Table — Senior
*Maîtrise : classements ITTF · analyse par sets/manches · styles de jeu · Bo5/Bo7*

### ⚠️ Protocole Anti-Hallucination
- Format obligatoire : `valeur (URL complète)` — ex : `ITTF #45 (ittf.com/rankings/player/12345)`
- Sans URL complète → **[MANQUANT]** | URL générique sans chemin → **INTERDIT**
- Jamais inventer · Jamais estimer

---

## PHASE 1 — COLLECTE WEB
> Recherche web obligatoire pour chaque ligne. Toute cellule sans URL complète → **[MANQUANT]**.

### 1.1 Profil & Classement
| Donnée | Joueur A | Joueur B |
|---|---|---|
| Classement ITTF mondial | | |
| Classement national | | |
| Best ranking | | |
| Âge | | |
| Main / latéralité | | |
| Style (offensif/défensif/allround) | | |
| Revêtement (picots, anti-spin…) | | |

> Sources : ittf.com · tabletennis.guide · flashscore.com · tt-ratings.com

### 1.2 Stats & Résultats Récents
| Donnée | Joueur A | Joueur B |
|---|---|---|
| Bilan W-L saison | | |
| Bilan W-L 3 derniers mois | | |
| Bilan vs top 20 / top 50 | | |
| % sets gagnés saison | | |
| Matchs gagnés 3-0/3-1 vs 3-2 *(ou 4-x)* | | |

### 1.3 Forme Récente *(8–10 derniers matchs)*
| Date | Adversaire (rang) | Score | Tournoi | Source URL |
|---|---|---|---|---|
**Joueur A :**

**Joueur B :**

> Tendance : en progression / stable / en baisse ?

### 1.4 H2H
| Date | Vainqueur | Score | Tournoi | Source URL |
|---|---|---|---|---|

> Domination d'un joueur ou équilibre ? Impact du style de jeu dans le H2H ?

### 1.5 Contexte
| Facteur | A | B | Justification |
|---|---|---|---|
| Enjeu / phase tournoi | | | |
| Fatigue (matchs enchaînés dans la journée) | | | |
| Adaptation (salle, table, balles) | | | |
| Mental / historique confrontations | | | |
| Expérience (JO, Mondiaux) | | | |

### 1.6 Cotes ParionsSport — Collecte complète
> Chercher sur **parionssport.fdj.fr** — relever **TOUTES** les cotes disponibles.

| Marché | Sélection | Cote | P.impl |
|---|---|---|---|
| Vainqueur | A / B | | |
| Handicap sets | −1.5/+1.5 · −2.5/+2.5 | | |
| Score exact sets | 3-0/3-1/3-2/0-3/1-3/2-3 *(ou 4-x)* | | |
| Over/Under sets | O/U 3.5 · O/U 4.5 | | |
| Over/Under points | Toutes lignes dispo | | |
| Vainqueur set 1 | A / B | | |
| Autres marchés | Tout ce qui est dispo | | |

> URL complète de la page. Non proposé → **[NON PROPOSÉ]**

---

### ✅ Vérification Croisée *(obligatoire)*
1. Lister **toutes** les URLs consultées *(complètes, pas les domaines)*
2. Pour chaque URL : résumé de ce qui a été trouvé
3. Cellule sans URL complète → **[MANQUANT]**
4. Classement ITTF cohérent avec les résultats récents ?
5. % sets gagnés entre 30–80% ? Sinon → **[SUSPECT]**
6. Dates H2H vérifiées jour/mois/année ?

### 🎯 Score de Confiance (/25)
| Critère | /5 | Score | Justification |
|---|---|---|---|
| URLs complètes (pas domaines) | /5 | | |
| Valeurs concrètes (pas d'estimation) | /5 | | |
| Recoupement 2+ sources concordantes | /5 | | |
| Fraîcheur < 7j (forme/condition) | /5 | | |
| Sections remplies | /5 | | |

**TOTAL : __ / 25** — Si < 21 : lister les [MANQUANT] par impact décroissant avant Phase 2.

===PHASE2===

## PHASE 2 — ANALYSE MATHÉMATIQUE
> Utiliser **UNIQUEMENT** les données Phase 1. [MANQUANT] → exclure du calcul.

### 2.1 Notation Multicritères (0–100)
*Échelle : 95–100 = top 10 mondial | 80–94 = top 50 | 65–79 = top 100 | 50–64 = sous top 100 | <50 = amateur*

| Critère | Poids | Note A | Note B | Source |
|---|---|---|---|---|
| Classement ITTF + Elo | 25% | | | |
| Bilan W-L saison | 15% | | | |
| Forme récente L8–10 | 25% | | | |
| H2H | 15% | | | |
| Style de jeu (avantage/désavantage) | 10% | | | |
| Contexte / mental / expérience | 10% | | | |

Ajustements :
- Fatigue (3+ matchs dans la journée) : × 0,90–0,95
- Matchup style (picots vs offensif) : +/− 5 pts

**Score_A = ∑(poids × note_A) / 100 = ___**
**Score_B = ∑(poids × note_B) / 100 = ___**

---

### 2.2 Probabilités & Estimation Sets
```
P(A gagne un set) = 1 / (1 + e^(−k × delta))   k=5 (Bo5)  ou  k=6 (Bo7)

Bo5 (premier à 3 sets) :
P(3-0) = P(A)³             P(0-3) = P(B)³
P(3-1) = C(3,1)×P(A)³×P(B)    P(1-3) = C(3,1)×P(B)³×P(A)
P(3-2) = C(4,2)×P(A)³×P(B)²   P(2-3) = C(4,2)×P(B)³×P(A)²

Bo7 (premier à 4 sets) :
P(4-0)=P(A)⁴  P(4-1)=C(4,1)×P(A)⁴×P(B)  P(4-2)=C(5,2)×P(A)⁴×P(B)²  P(4-3)=C(6,3)×P(A)⁴×P(B)³
(idem symétrique pour B)
→ Renormaliser pour somme = 100%
```
**P(victoire A) = ___% | P(victoire B) = ___%** *(somme = 100%)*

---

### 2.3 Scan Complet — Cotes Phase 1
> Une ligne par marché collecté en 1.6. Cotes inventées → **INTERDIT**. [NON PROPOSÉ] → ne pas lister.

| Marché | P.simulée | Cote PS | P.impl | EV | Kelly |
|---|---|---|---|---|---|

`EV = (P_sim × Cote) − 1` | `Kelly = (P_sim × Cote − 1) / (Cote − 1)`

---

### 2.4 Filtre — Dérivés P ≥ 70% & EV > 0
| Marché | P.sim | Cote | EV | Kelly | Robuste ? |
|---|---|---|---|---|---|

Test sensibilité −10% sur les scores : **Robuste** = value maintenue | **Fragile** = value disparaît
> Si aucun : baisser à 65% → **[SEUIL ABAISSÉ]**

---

### 🏆 Recommandation Finale
**Match :** [A] vs [B] | **Tournoi :** ___ | **Format :** Bo5 / Bo7

**MEILLEUR PARI DÉRIVÉ** *(P ≥ 70% + Value)* :
> Sélection : ___ @ cote ___ | P.sim : ___% | EV : +___ | Kelly : ___% | Robustesse : ___

Facteurs clés : 1. ___ 2. ___ 3. ___
Risques : 1. ___ 2. ___
Confiance données : __ /25

**Autres dérivés qualifiés** *(P ≥ 70% + EV > 0 — par EV décroissante)* :
> Si aucun : *"Aucun dérivé ne combine P≥70% et EV>0 → NE PAS JOUER."*
