# Obelix AI Navigatie Labo

## Inleiding
Dit rapport documenteert de ontwikkeling en training van een autonome agent, genaamd "Obelix", binnen een gesimuleerde Unity-omgeving middels de ML-Agents toolkit. Het primaire doel van dit onderzoek is het evalueren van het leervermogen van een agent bij het uitvoeren van sequentiële taken: het detecteren, ophalen en afleveren van objecten (Menhirs) naar specifieke doellocaties. Het rapport is bedoeld voor ontwikkelaars en onderzoekers op het gebied van kunstmatige intelligentie en biedt inzicht in de configuratie en de behaalde resultaten van verschillende trainingsfasen.

## Methoden
Het systeem is opgebouwd rondom twee primaire componenten binnen de Unity-omgeving:

### 1. Primaire Componenten
Behaviour Parameters: Deze component definieert hoe de agent beslissingen neemt. Er is gebruikgemaakt van een Vector Observation Space (grootte 2) voor het bijhouden van de status (object in bezit) en de progressie van de taak. De agent beschikt over twee Discrete Action Branches (elk grootte 3) voor beweging en rotatie.

Obelix Agent: De agent is verantwoordelijk voor de interactie met de omgeving. Voor de visuele waarneming is een Ray Perception Sensor 3D toegevoegd, die objecten detecteert via specifieke tags ("Menhir" en "Destination").

### 2. Override Methods
De logica van de agent is geïmplementeerd via de volgende kernfuncties:

* OnEpisodeBegin(): Initialiseert de omgeving door objecten op willekeurige posities te spawnen en de status van de agent te resetten.
* CollectObservations(VectorSensor sensor): Verzamelt numerieke data over de status van de agent, zoals of er een object wordt gedragen en welk percentage van de totale taak is voltooid.
* OnActionReceived(ActionBuffers actionBuffers): Vertaalt de beslissingen van het neurale netwerk naar fysieke bewegingen en kent beloningen of straffen toe op basis van gedrag en tijdsverloop.
* Heuristic(in ActionBuffers actionsOut): Maakt handmatige besturing via het toetsenbord mogelijk voor testdoeleinden.

## Resultaten
De trainingsresultaten zijn geanalyseerd via TensorBoard, waarbij twee verschillende scenario's zijn getoetst:

* Scenario A (Eén object): De initiële test met één Menhir (weergegeven door de groene grafiek Obelix_v1) toont een snelle stijging in de cumulatieve beloning. Na ongeveer 40.000 stappen stabiliseert de score zich rond een waarde van 2.0.
<img width="1918" height="877" alt="image" src="https://github.com/user-attachments/assets/9d7a4a9d-de59-4bef-90d2-bd5b7946df60" />

* Scenario B (Zes objecten): Bij de uitgebreide test met zes Menhirs (weergegeven door de oranje grafiek Obelix_v2) bereikt de cumulatieve beloning een waarde boven de 6.0 na 500.000 stappen. De Episode Length grafiek vertoont een significante daling na 200.000 stappen, wat duidt op een toename in efficiëntie. De Policy Loss vertoont fluctuaties tussen 0.022 en 0.027, terwijl de Value Loss een stijgende lijn vertoont naarmate de agent meer complexe beloningen identificeert.
<img width="1597" height="862" alt="image" src="https://github.com/user-attachments/assets/c793ad43-9a37-4ebb-a805-e0adf0e47cff" />
<img width="1043" height="805" alt="image" src="https://github.com/user-attachments/assets/281f0fb8-e998-4045-aa4c-e01f1055a7ba" />
<img width="1591" height="857" alt="image" src="https://github.com/user-attachments/assets/3bbec719-3041-495c-b154-f97c970a2df5" />

*Scenario C (Afstraffing voor te blijven hangen): Door dat ik een afstraffing heb toegevoegd aan het script, leert het Obelix_v3 model veel sneller.
### Cumulative Reward:
<img width="1582" height="732" alt="image" src="https://github.com/user-attachments/assets/cb987c1f-e82e-40f1-9f5b-b5e745cb7f83" />
Bij 520.000 stappen behaalt Obelix_v3 een aanzienlijk hogere gemiddelde beloning van 708,32, vergeleken met 526,81 voor Obelix_v2. Dit is een stijging van ongeveer 34% in totale verzamelde beloningen per episode.

### Episode Length:
<img width="1581" height="557" alt="image" src="https://github.com/user-attachments/assets/f7b2af37-fc95-4db8-a23a-2e0b8fb4e419" />
De grafiek laat zien dat Obelix_v3 de taken sneller voltooit. Waar Obelix_v2 vaak langer blijft hangen in episodes (wat duidt op het 'vastlopen' bij balken), vertoont v3 een stabielere neerwaartse trend in de tijd die nodig is om de 6 menhirs af te leveren.

### Policy Loss: 
<img width="1580" height="555" alt="image" src="https://github.com/user-attachments/assets/a55c8397-ff38-4415-b1bc-08a999c40367" />
De Policy Loss van v3 is met 0,0228 iets lager en stabieler dan die van v2 (0,0245). Dit wijst erop dat de agent meer zelfvertrouwen heeft in de gekozen acties om het doel te bereiken.
### Value Loss: 
We zien een hogere Value Loss bij v3 (0,0898) vergeleken met v2 (0,0629). Dit is logisch en zelfs positief in deze context: omdat we de beloningsstructuur complexer hebben gemaakt met afstandsbeloningen, is het voor de agent in het begin lastiger om de exacte waarde van een situatie te voorspellen, maar dit leidt uiteindelijk tot een veel beter eindresultaat.

## Conclusie
Op basis van de observaties kan worden geconcludeerd dat de agent succesvol in staat is om complexe, sequentiële taken aan te leren. De stabilisatie van de cumulatieve beloning bij de waarde 6.0 in het tweede scenario bewijst dat de agent consistent alle zes de objecten aflevert. De afname in episodelengte suggereert dat de agent niet alleen de taak voltooit, maar ook optimaliseert voor snelheid om tijdsgerelateerde straffen te minimaliseren. Het gebruik van Ray Perception Sensors blijkt essentieel voor navigatie in omgevingen met een hoge densiteit aan objecten.

## Referenties
Unity Technologies (2024) ML-Agents Toolkit Documentation. https://github.com/Unity-Technologies/ml-agents

Juliani, A., et al. (2018) Unity: A General Platform for Intelligent Agents. arXiv preprint arXiv:1809.02627.
