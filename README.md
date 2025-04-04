Bienvenu dans le projet LivinParis, réalisé par Adam Sarehale et Clément Sanchez

Rapport du Rendu 04/04/2025

1-Schéma Entité-Association et Script SQL
  -Le schéma EA a été modifié et améliorer pour mieux correspondre au cahier des charges.
  -Le script SQL a été modifié pour correspondre au nouveau schéma EA et fournir la base de données et les tables nécesseraires. 
De plus, les tables ont été plus peuplé pour rendre la base de données plus fonctionnelle et compréhensible. Des requêtes ont été ajoutés pour pouvoir par exemple afficher les commandes selon une période de temps, afficher la moyenne des prix des commandes, afficher la moyenne des comptes clients et d'autres encore.

Pour pouvoir accéder aux informations de notre serveur SQL:
Server=127.0.0.1
Port=3306
Database=site
User Id=root
Password=Mrick88rick!
SslMode=None;

2-Solution C#

On a comparé les trois algorithmes : Dijkstra, Bellman-Ford, et Floyd-Warshall. On a trouvé que Dijkstra était le meilleur dans ce cas, car il est beaucoup plus rapide et efficace quand il n'y a pas de poids négatifs, comme c'est le cas ici. En plus, on a modifié Dijkstra pour qu'il affiche aussi le chemin parcouru, ce qui est très utile dans les applications pratiques. Tandis que Bellman-Ford est plus lent et qu'on n'a pas besoin de gérer des poids négatifs, Floyd-Warshall est plus adapté pour des graphes très grands avec de nombreux sommets, mais il est aussi moins performant que Dijkstra pour notre cas précis. On a donc modifié Dijkstra pour afficher les chemins parcouru. 
