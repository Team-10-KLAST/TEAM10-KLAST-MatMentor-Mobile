using System.Collections.Generic;

namespace MatMentor.Mobile.Data;

public static class ExercisePrototypeData
{
    // Returns temporary exercise data for the selected topic.
    public static List<string> GetExercises(string topicName)
    {
        List<string> exercises = topicName switch
        {
            "Brøk, decimaltal og procent" =>
            [
                "På et typisk døgn er Anna i skole i 7 timer, hun sover i 8 timer, og hun har 9 timers fritid. Hvor stor en brøkdel af et typisk døgn udgør Annas fritid?",

                "Udgiften til el til én LED-pære er 1 kr. om måneden, mens en halogenpære koster 5 kr. om måneden i el. Hvor mange procent er udgiften mindre for LED-pæren?",

                "En sort elscooter koster 19.998 kr. Den er på tilbud og 15 % billigere. Hvad koster den på tilbud?",

                "Anna tjente 2.134,08 kr. og skal betale 8 % i arbejdsmarkedsbidrag. Hvor mange penge får hun udbetalt?",

                "En biografbillet koster 89,50 kr. Liva kan få 35 % rabat ved at melde sig ind i en filmklub. Hvad skal hun betale for billetten?"
            ],

            "Handelsregning" =>
            [
                "En LED-pære koster 59,50 kr. og en halogenpære koster 34,00 kr. Hvor stor er prisforskellen?",

                "Et 10-turskort til børn koster 219 kr., og et 10-turskort til voksne koster 465 kr. Hvor stor er prisforskellen?",

                "En enkeltbillet til børn koster 25 kr. Et 10-turskort koster 219 kr. Hvor mange penge sparer Magnus pr. tur ved at købe et 10-turskort i stedet for 10 enkeltbilletter?",

                "En grøn elscooter koster 12.348 kr., og en hvid elscooter koster 9.989 kr. Hvor meget dyrere er den grønne elscooter?",

                "En sort elscooter koster 19.998 kr. og er sat 15 % ned. Hvad koster den på tilbud?"
            ],

            "Tid" =>
            [
                "Der er 20 døgn til juleaften. Hvor mange timer er der på 20 døgn?",

                "Hvor mange sekunder er der på 1 time?",

                "Hvor mange minutter er der på n timer?",

                "En LED-pære har en levetid på 15.000 timer. Hvor mange år kan den holde, hvis den bruges 3 timer om dagen?",

                "Et stearinlys er 22 cm langt ved start. Efter 20 minutter er det 21 cm, efter 40 minutter 20 cm, efter 60 minutter 19 cm og efter 80 minutter 18 cm. Hvor langt vil lyset være efter 120 minutter?"
            ],

            _ => []
        };

        return exercises;
    }
}