# lab-02-floffos124

Rapport: Onderzoek naar Autonome Sequentiële Taakvoltooiing middels Reinforcement Learning

# Inleiding
Het doel van dit rapport is het vastleggen van de resultaten van een onderzoek naar het leervermogen van een software-agent in een gesimuleerde 3D-omgeving. Dit onderzoek is bedoeld voor ontwikkelaars en onderzoekers op het gebied van Machine Learning. Er wordt onderzocht of een neuraal netwerk in staat is om via vallen en opstaan (Reinforcement Learning) een complexe fysieke interactie aan te leren: het navigeren naar een object en dit object vervolgens gericht naar een bestemmingszone duwen.

# Methoden
Het onderzoek maakt gebruik van de Unity ML-Agents toolkit (Juliani et al., 2018). De opstelling bestaat uit twee primaire componenten:

Behaviour Parameters: De agent wordt aangestuurd door het Proximal Policy Optimization (PPO) algoritme. De Vector Observation Space is ingesteld op 9 (XYZ-posities van agent, target en zone). De actieruimte bestaat uit 2 continue variabelen voor beweging en rotatie.

Agent (CubeAgentPushShaped): De agent is uitgerust met een Ray Perception Sensor 3D (bereik 70°, 3 stralen) voor visuele detectie van de tags "Target" en "Zone". De agent moet een fysieke kracht uitoefenen op de Rigidbody van het doelenblok om verplaatsing te realiseren.

Override Methods van de Agent:
OnEpisodeBegin: Reset de posities van de agent en het doelenblok bij aanvang van elke sessie om variatie in de trainingsdata te waarborgen.
CollectObservations: Registreert de exacte ruimtelijke coördinaten van de agent, het blok en de zone voor het neurale netwerk.
OnActionReceived: Vertaalt de netwerk-output naar fysieke krachten. Hierbij wordt Distance-based Reward Shaping toegepast: de agent ontvangt een kleine beloning (+0.001) als de afstand tussen blok en zone verkleint, wat dient als "begeleidend signaal" tijdens het leerproces.

# Resultaten
Bij de analyse van de geoptimaliseerde trainingssessie (v5) worden de volgende observaties gedaan:

Cumulative Reward: De grafiek toont een duidelijke stijgende trend. Waar de beloning aanvankelijk stabiel bleef op -1.0, klimt deze na circa 100.000 stappen richting de +0.4 tot +0.7. Dit duidt op een succesvolle voltooiing van de taak in een groeiend percentage van de episodes.
Episode Length: De gemiddelde duur van een episode stabiliseert zich rond de 150-200 stappen. Dit is een aanzienlijke verkorting ten opzichte van de eerdere 326 stappen, wat wijst op een efficiënter navigatiepad naar de zone.
Value Loss: Na een initiële piek vertoont de Value Loss een scherpe daling en stabiliseert rond de 0.005, wat duidt op een zeer hoge betrouwbaarheid van de waardevoorspellingen van het model.
Gedrag: Visuele observatie in Unity toont aan dat de agent doelgericht achter het blokje manoeuvreert om de vector richting de groene zone te maximaliseren.

# Conclusie
Op basis van de observaties kan worden geconcludeerd dat de overstap van Sparse Rewards naar Shaped Rewards (afstand-gebaseerd) cruciaal is voor het oplossen van fysieke interactietaken. De agent heeft succesvol de correlatie geleerd tussen de eigen positie, de positie van het object en de relatieve afstand tot het doel. De significante stijging in de cumulatieve beloning en de verkorting van de episode-duur bewijzen dat het neurale netwerk niet alleen de taak begrijpt, maar deze ook optimaliseert op snelheid en nauwkeurigheid.

# Referenties
In dit onderzoek is gebruikgemaakt van concepten uit de officiële documentatie en wetenschappelijke basis van Unity ML-Agents:

(Juliani et al., 2018)

Juliani, A., Berges, V., Vckay, E., Gao, Y., Henry, H., Mattar, S., Lange, D. (2018). Unity: A General Platform for Intelligent Agents. arXiv preprint arXiv:1809.02627.
