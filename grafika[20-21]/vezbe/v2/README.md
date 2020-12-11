# Vezbe2

Orijentacija je definisana redosledom definisanja tacaka.

## GL_TRIANGLE_STRIP primitiva

  - svaki drugi trougao ima istu orijentaciju, open gl to resava tako sto uzima svaki parni trougao i menja mu prva dva verteksa 
![image](https://user-images.githubusercontent.com/45834270/97884639-9de62300-1d26-11eb-9353-05030b2ac20d.png)


## GL_TRIANGLE_FAN primitiva

  - prva tri verteksa (tacke) definisu prvi trougao
  - svaki novi trougao se dobija dodavanjem nove tacke
  - nema problema sa orijentacijom

## GL_QUAD_STRIP primitiva

  - prvo se spajaju prva i druga tacka, potom druga sa cetvrtom a onda cetvrta sa trecom i na kraju treca i prva zatvaraju taj cetvorougao
  - ista orijentacija za svaki quad
  - istu orijentaciju imamo, zato sto se za svaki cetvorougao zamene poslednja dva verteksa (tacke)

![image](https://user-images.githubusercontent.com/45834270/97888863-ee13b400-1d2b-11eb-948d-ee206bdfd188.png)

![image](https://user-images.githubusercontent.com/45834270/97888739-c7557d80-1d2b-11eb-8a13-8d69125a6f98.png)

## Line stipple

![image](https://user-images.githubusercontent.com/45834270/97889278-76925480-1d2c-11eb-8e90-c25bd21d6486.png)

Rezim rada sa isprekidanim linijama. Sa desna na levo gledamo, 1 - prikazi piksel, 0 - nemoj da ga prikazes. Faktor kaze koliko piksela da se prikaze. Ako imamo sledecu situaciju 011 i faktor 5. Skroz desni bit kaze prikazi sledecih 5 piksela, onda drugi bit kaze prikazi sledecih 5 piksela i poslednji bit (0 bit) kaze nemoj da prikazes sledecih 5 piksela. Kratko receno, taj faktor govori koliko jedan bit oznacava piksela.

## Test pitanja

### Primitiva gl_triangle_strip

- svaka dva imaju razlicitu orijentaciju 
- svaki drugi trougao ima istu orijentaciju
- ali opengl regulise tu pricu tako sto uzima svaki parni trougao i menja mu prva dva verteksa(tacke)
  
## Primeri pitanja na testu 
  
  - za quadric objekte reci koji postoje atributi, sta svaki od njih radi 
  - koliko tacaka je potrebno da definisemo n trouglova u triangle fan tipu primitive : 2 + n tacaka
  - pitanje vezano za projekcije 
  - funkcija za definisanje kamere, odnosno point of view u opengl-u (viewport metoda)
  
## Zakljucak

Svaku primitivu je potrebno znati jako dobro ! Skripta je sasvim dovoljna za test. Test se radi 10 minuta gde imamo 2 pitanja.

</br></br></br>

## Matrice transformacije 

![image](https://user-images.githubusercontent.com/45834270/97891893-a7c05400-1d2f-11eb-9e3e-239db9762a04.png)


  - Kada kreiramo objekat, vertex, sta god, on se kreira u local space (lokalni kordinatni sistem) koji se nalazi u centru globalnog kordinatnom sistema koji predstavlja kordinatni sistem citave nase scene.   
  - Kada smo ga kreirali u model matrixu koristimo rotaciju, skaliranje, translaciju da bi rasporedili taj objekat na odgovarajuce mesto u globalnoj sceni.
  - View matrix se koristi da se definise point of view ( odakle gledamo scenu tj iz koje perspektive  )
  - Onda vrsimo projection matrix da bi definisali na koji nacin zelimo da se izvrsi 3d projekcija na 2d ekran odnosno na klip ravni nase projekcije tj prostora. 
  - Na kraju se vrsi viewport transform koji ustvari sve te kordinate transformise u screen space, odnosno u kordinate na ekranu i na kraju dobijamo neki prikaz kocke, piramide, sta god.
  
Skaliranje u sebi sadrzi translaciju. Da bi to resili, vratimo teziste objekta u kordinatni pocetak, potom odradimo skaliranje i onda uradimo istu translaciju da bi vratili objekat na prvobitno mesto i na kom je bio.
