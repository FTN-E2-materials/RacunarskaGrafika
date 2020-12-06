# Vezbe5

<details>
  <summary> Paralelna projekcija </summary> <br>
  
  ![image](https://user-images.githubusercontent.com/45834270/100785856-8a0b0b00-3411-11eb-97b1-e6d2374bd91f.png)
</details>

<details>
  <summary> Perspektivna projekcija </summary> <br>

  ![image](https://user-images.githubusercontent.com/45834270/100785970-b9ba1300-3411-11eb-85ce-1d9b31186825.png)
  ![image](https://user-images.githubusercontent.com/45834270/100785982-bf175d80-3411-11eb-8140-c61c71b6a9c6.png)
</details>


<details>
  <summary> Tekst </summary> <br>

  - sto se tice 3D teksta, on se pomera sa scenom
  - sto se tice 2D teksta, objekti mogu preci preko njih, nije po deafultu da su oni nalepljeni na scenu, tkd to moramo resiti sa **depth testingom**
  - najsigurnija opcija je da:
    - iscrtamo tekst poslednji po redu 
    - i pre nego sto iscrtamo tekst, iskljucimo depth testing i tako znamo da ce se pregaziti svi pikseli na tim mestima gde se nalazi tekst
    - nakon sto iscrtamo tekst, samo vratimo depth testing ili ga vratimo kad isrtavamo novi frejm 

</details>
