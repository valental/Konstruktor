# Konstruktor
Projekt iz kolegija Računarski praktikum 3

Zadatak:

- Napravite aplikaciju koja omogućuje sastavljanje konstrukcija od manjih dijelova. Dijelovi su oblika kvadra i razlikuju se po veličini. Nalaze se u izborniku, a korisnik unošenjem dimenzija može dodati još dijelova u izbornik. Na najnižoj razini dozvoljeno je staviti bilo koji dio. Na višoj razini dio se može staviti samo ako se dva suprotna ruba mogu kompletno položiti na dijelove s niže razine. Preciznije, postoji neka osnovna duljina "o" i svaki dio ima duljinu, širinu i visinu koje su višekratnici te osnovne duljine. Nadalje, ako bazu dijela poistovijetimo s matricom m×n (baza dakle ima dimenzije "mo×no"), onda se moraju moći potpuno položiti prvi i n-ti stupac ili prvi i m-ti redak. Dijelovi uvijek moraju stajati vodoravno, a vrhovi im imaju cjelobrojne koordinate. 

- Program u središnjem panelu u početku pokazuje tlocrt konstrukcije, ali može se mijenjati pogled, tj, konstrukcija bi se trebala moći vidjeti sa svih pet vidljivih strana. 

- Taj pogled bi cijelo vrijeme trebao biti omogućen u dodatnim umanjenim panelima. Pogled je dakle uvijek dvodimenzionalan.

- Dodavanje nekog dijela na konstrukciju se može obaviti povlačenjem dijela iz izbornika dijelova te ispuštanjem "blizu" mjesta gdje ga želimo postaviti ako je to postavljanje legalno. 

- Drugi način je uključivanjem mreže na konstrukciju te izborom dijela i odabirom neke točke na mreži. 

- Dodavanje se može vršiti neovisno o tome koji je pogled u središnjem panelu (u slučaju kad nemamo tlocrt u središnjem panelu treba paziti na dubinu postavljanja). Pritom konstrukcija može biti u jednoj boji ili se mogu posebno obojati dijelovi na istoj razini. 

- Omogućite i poništavanje nekoliko zadnjih poteza, zatim micanje zadanog dijela s konstrukcije, uklanjanje cijelog nivoa, spremanje konstrukcije i ponovno učitavanje, tj. mijenjanje već postojeće konstrukcije. 
