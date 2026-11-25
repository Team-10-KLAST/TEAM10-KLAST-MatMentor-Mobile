// File: MatMentor.Mobile/Data/MissionPresentationCatalog.cs
namespace MatMentor.Mobile.Data;

public sealed record MissionPresentation(string Introduction, string ImageFile, int Grade, string Interest);

public static class MissionPresentationCatalog
{
    private static readonly IReadOnlyDictionary<string, MissionPresentation> Entries =
        new Dictionary<string, MissionPresentation>(StringComparer.Ordinal)
        {
            ["Mission: Åbn skolens pop-up-café!"] = new("Din klasse skal åbne en lille pop-up-café på skolen. Du hjælper med at holde styr på snacks, tilbud og indkøb, så der er nok til alle, og pengene bliver brugt fornuftigt.", "mission_set_003.png", 7, "Standard"),
            ["Mission: Få skolens genbrugs- og designmarked til at lykkes!"] = new("Din klasse arrangerer et genbrugs- og designmarked på skolen. Gamle ting skal have nye ejere, og I sælger også ting, som I selv har designet. Overskuddet skal bruges til at gøre skolegården hyggeligere.", "mission_set_004.png", 8, "Standard"),
            ["Mission: Få skolens minifestival til at løbe rundt!"] = new("Elevrådet arrangerer en minifestival med musik, aktiviteter og madboder. Du er med på økonomiholdet og skal hjælpe med at planlægge indkøb, sammenligne tilbud og sikre, at festivalen får råd til nye aktiviteter.", "mission_set_005.png", 9, "Standard"),
            ["Mission: Gør klubben klar til weekendens fodboldstævne!"] = new("Din lokale fodboldklub arrangerer et stævne. Du hjælper med at holde styr på indkøb, kiosksalg og udstyr, så klubben er klar, når spillerne og tilskuerne kommer.", "mission_set_006.png", 7, "Fodbold"),
            ["Mission: Få holdets udebanetur til at hænge sammen!"] = new("Fodboldholdet skal til et stævne i en anden by. Du hjælper klubben med at finde gode indkøb, holde budgettet og skaffe penge til turen.", "mission_set_007.png", 8, "Fodbold"),
            ["Mission: Planlæg klubbens talentcup uden underskud!"] = new("Fodboldklubben arrangerer en talentcup med hold fra flere byer. Du hjælper med kapacitet, indkøb, priser og overskud.", "mission_set_008.png", 9, "Fodbold"),
            ["Mission: Gør rideklubben klar til stævnedagen!"] = new("Rideklubben gør klar til en stævnedag med ryttere og gæster. Du hjælper med at købe udstyr, holde styr på kiosken og finde ud af, hvad klubben har råd til.", "mission_set_009.png", 7, "Ridning"),
            ["Mission: Få ridelejrens økonomi til at hænge sammen!"] = new("Rideklubben planlægger en ridelejr og skal have økonomien til at hænge sammen. Du hjælper med at sammenligne tilbud, lægge budget og undersøge, hvad klubben tjener på sine aktiviteter.", "mission_set_010.png", 8, "Ridning"),
            ["Mission: Planlæg rideklubbens store forårsstævne!"] = new("Rideklubben arrangerer sit store forårsstævne. Du er med til at undersøge billetter, rabatter, indkøb og priser, så klubben kan planlægge stævnet uden at miste overblikket over økonomien.", "mission_set_011.png", 9, "Ridning"),
            ["Mission: Planlæg klassens aktivitetsdag!"] = new("Klassen planlægger en aktivitetsdag med spil, kreative værksteder og pauser. Du hjælper med at fordele materialer og aktiviteter ved at regne med brøker, decimaltal og procent.", "mission_set_012.png", 7, "Standard"),
            ["Mission: Gør skolens udstilling klar!"] = new("Skolen forbereder en udstilling med plakater, dekorationer og gæster. Du hjælper med at holde styr på, hvor meget der er gjort klar, og hvordan mængder og andele hænger sammen.", "mission_set_013.png", 8, "Standard"),
            ["Mission: Gør skolens miljøuge målbar!"] = new("Skolen holder miljøuge og vil undersøge resultaterne med tal. Du hjælper med at sammenligne udvikling, finde andele og forstå, hvordan procentændringer påvirker det samlede resultat.", "mission_set_014.png", 9, "Standard"),
            ["Mission: Hold styr på dagens fodboldtræning!"] = new("Fodboldholdet har en træningsdag med forskellige øvelser og små udfordringer. Du hjælper træneren med at bruge brøker, decimaltal og procent til at forstå dagens tal.", "mission_set_015.png", 7, "Fodbold"),
            ["Mission: Analyser fodboldakademiets sæson!"] = new("Fodboldakademiet samler tal fra træning og kampe. Du hjælper med at undersøge løbedistancer, skud og billetter og finde ud af, hvad tallene fortæller.", "mission_set_016.png", 8, "Fodbold"),
            ["Mission: Vurder holdets sæsonstatistik!"] = new("Fodboldklubben vil forstå sin sæson ved hjælp af statistik. Du undersøger kampresultater, afleveringer og ændringer i tilskuer- og medlemstal med brøker, decimaltal og procent.", "mission_set_017.png", 9, "Fodbold"),
            ["Mission: Hjælp til på rideskolens stalddag!"] = new("Rideskolen har stalddag, og der er mange ting, der skal fordeles og gøres klar. Du hjælper med at bruge brøker, decimaltal og procent til at holde styr på ponyer, foder og udstyr.", "mission_set_018.png", 7, "Ridning"),
            ["Mission: Brug tallene fra rideskolens træningsdag!"] = new("Rideskolen arrangerer en træningsdag for forskellige ridehold. Du hjælper med at forstå deltagertal, rideture og tidsfordeling ved at regne med brøker, decimaltal og procent.", "mission_set_019.png", 8, "Ridning"),
            ["Mission: Analyser rideklubbens stævnetal!"] = new("Rideklubben har samlet tal fra sine stævner og træninger. Du hjælper med at undersøge resultater, andele og procentvise ændringer, så tallene kan bruges til at forstå udviklingen.", "mission_set_020.png", 9, "Ridning"),
            ["Mission: Få skolens aktivitetsdag til at passe!"] = new("Skolen holder aktivitetsdag med værksteder, pauser og en bustur. Du hjælper med at beregne, hvor lang tid aktiviteterne tager, så dagens program kan hænge sammen.", "mission_set_021.png", 7, "Standard"),
            ["Mission: Få værkstedernes tidsplan til at fungere!"] = new("Klassen skal gennemføre flere værksteder efter en tidsplan. Du hjælper med at beregne varigheder, skift og pauser, så der er tid til alle aktiviteterne.", "mission_set_022.png", 8, "Standard"),
            ["Mission: Planlæg skolens studietur uden forsinkelser!"] = new("Klassen skal på studietur, og transporten skal passe med dagens program. Du hjælper med at regne på rejsetid, pauser og tidsgrænser – også når en tur fortsætter efter midnat.", "mission_set_023.png", 9, "Standard"),
            ["Mission: Få holdets træningslørdag til at passe!"] = new("Fodboldholdet holder træningslørdag med øvelser og små kampe. Du hjælper med at få træningstider og pauser til at passe, så spillerne ved, hvad der skal ske.", "mission_set_024.png", 7, "Fodbold"),
            ["Mission: Lav en præcis tidsplan for træningslejren!"] = new("Holdet skal på træningslejr, hvor flere træningspas skal passe ind i dagens program. Du hjælper med at beregne forskelle i træningstid, pauser og den tid, der er tilbage.", "mission_set_025.png", 8, "Fodbold"),
            ["Mission: Styr tid og transport til klubbens aftencup!"] = new("Fodboldklubben deltager i en aftencup med kampe og transport. Du hjælper med at beregne rejsetid og kamptid, så holdet kan planlægge dagen og komme sikkert hjem.", "mission_set_026.png", 9, "Fodbold"),
            ["Mission: Få stalddagens program til at passe!"] = new("Rideskolen skal have styr på stalddagens aktiviteter. Du hjælper med at beregne tid til klargøring, ridning og pauser, så rytterne kan følge programmet.", "mission_set_027.png", 7, "Ridning"),
            ["Mission: Få rideskolens træningsdag til at gå op!"] = new("På rideskolens træningsdag deler flere hold ridebane og udstyr. Du hjælper med at beregne varigheder og overgange, så træningen kan planlægges uden tidskollisioner.", "mission_set_028.png", 8, "Ridning"),
            ["Mission: Planlæg ridestævnets tidskrævende transport!"] = new("Rideklubben arrangerer et stævne med transport, konkurrencer og pauser. Du hjælper med at regne på tidsforbruget, også når transporten går over midnat, så stævnedagens program hænger sammen.", "mission_set_029.png", 9, "Ridning"),
        };

    // Looks up approved visual content by the exact title returned by the current backend.
    public static MissionPresentation? Find(string? title)
    {
        return title is not null && Entries.TryGetValue(title, out MissionPresentation? mission)
            ? mission
            : null;
    }
}
