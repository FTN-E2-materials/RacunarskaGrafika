# Uvod

Teorijski testovi (koji se rade na odbrani kontrolnih tacki) nisu vezani za predavanja profesora. Postoji skripta za te testove i eventualno nesto sa vezbi sto je asistent spomenuo na vezbama je ono sto se pita na tim teorijskim testovima.

## Terminologija
  - **verteks**: jedna tacka u 3d sceni
  - **primitive/polygon**: zatvorena/otvorena polinija(linija, mnogougao, trougao i sl.)
  - **objekat/model**: 3D objekat izgradjen od primitiva
  - **fragment**: skup piksela istih prikaznih karakteristika
  - **pixel**: najmanja jedinica na prikaznom uredjaju
  - **framebuffer**: deo memorije u kojoj se cuva niz piksela koji predstavlja nasu scenu

## Napomena

U OpenGL se ne radi 3D modelovanje, vrlo je zahtevno (skoro pa ne moguce). Postoje softveri za modelovanje, napravimo u njima model i samo taj model ucitamo u OpenGL-u ( onda radimo neke transformacije nad njim, probavati da ih animiramo i slicno).


</br>
</br>

## Vezbe1

Da bi radili sa OpenGL u C# koristimo SharpGL i sledece reference.

![image](https://user-images.githubusercontent.com/45834270/97192720-77653c80-17a8-11eb-804e-e9d85eb2904a.png)

Njih mozemo dodati u projekat kroz NuGet packages:

![image](https://user-images.githubusercontent.com/45834270/97193001-cca14e00-17a8-11eb-942a-ede9025d852d.png)

I pomocu sledeceg taga ga dodajemo u wpf projekat:

![image](https://user-images.githubusercontent.com/45834270/97193375-391c4d00-17a9-11eb-8e45-bbada2caa645.png)

## Pitanje na KT1 - redovno pitanje !

Dobijemo neki deo koda. I mi treba da skiciramo sta ce se iscrtati na ekranu za taj deo koda.

Primer koda:

![image](https://user-images.githubusercontent.com/45834270/97197803-57d11280-17ae-11eb-8821-3f50965d73a2.png)



