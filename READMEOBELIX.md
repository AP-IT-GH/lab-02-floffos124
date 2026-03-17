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

* Scenario B (Zes objecten): Bij de uitgebreide test met zes Menhirs (weergegeven door de oranje grafiek Obelix_v2) bereikt de cumulatieve beloning een waarde boven de 6.0 na 500.000 stappen. De Episode Length grafiek vertoont een significante daling na 200.000 stappen, wat duidt op een toename in efficiëntie. De Policy Loss vertoont fluctuaties tussen 0.022 en 0.027, terwijl de Value Loss een stijgende lijn vertoont naarmate de agent meer complexe beloningen identificeert.

## Conclusie
Op basis van de observaties kan worden geconcludeerd dat de agent succesvol in staat is om complexe, sequentiële taken aan te leren. De stabilisatie van de cumulatieve beloning bij de waarde 6.0 in het tweede scenario bewijst dat de agent consistent alle zes de objecten aflevert. De afname in episodelengte suggereert dat de agent niet alleen de taak voltooit, maar ook optimaliseert voor snelheid om tijdsgerelateerde straffen te minimaliseren. Het gebruik van Ray Perception Sensors blijkt essentieel voor navigatie in omgevingen met een hoge densiteit aan objecten.

## Referenties
Unity Technologies (2024) ML-Agents Toolkit Documentation. https://github.com/Unity-Technologies/ml-agents

Juliani, A., et al. (2018) Unity: A General Platform for Intelligent Agents. arXiv preprint arXiv:1809.02627.
