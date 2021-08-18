using System.Threading.Tasks;
using ControlComponents.Core;

/// <summary>
/// Field-based asynchronous interface for communication between <see cref="MLOperationMode"/>
/// and <see cref="MLModel"/>, which may be realised via network communication in the future.
/// </summary>
/// <remarks>
/// < para />
/// Interface variable definitions.
/// | #  | I/O |   Name   |      Langname       |        Datentyp         |            Beschreibung             |
/// |----|-----|----------|---------------------|-------------------------|-------------------------------------|
/// | 1  | O   | OPMODE   | Operation Mode      | String                  | Aktuelle Fahrweis                   |
/// | 2  | O   | EXMODE   | Execution Mode      | String                  | Aktuelle Betriebsart                |
/// | 3  | O   | EXST     | Execution State     | String                  | Aktueller Betriebszustand           |
/// |----|-----|----------|---------------------|-------------------------|-------------------------------------|
/// | 4  | I   | MLSC     | ML State Change     | String                  | Rückmeldung Betriebszustandswechsel |
/// | 5  | O   | MLMODEL  | ML Model            | String                  | Auswahl des ML-Modells              |
/// | 6  | O   | MLOBSERV | ML Oberservation    | Float[1..*] \{ordered\} | Letzte Beobachtungen                |
/// | 7  | O   | MLENACT  | ML Enable Action    | Bool[1..*] \{ordered\}  | Maskierung möglicher Aktionen       |
/// | 8  | I   | MLDECIDE | ML Decision         | Float[1..*] \{ordered\} | Entscheidungen des ML-Modells       |
/// | 9  | O   | MLREWARD | ML Reward           | Float                   | Bewertung                           |
/// | 10 | O   | MLREWINC | ML Reward Increment | Bool                    | Inkrementelle / Absolute Bewertung  |
/// | 11 | I   | MLSTATS  | ML Stats            | String                  | Statistik des ML-Modells            |
/// < para />
/// Variables involed in standard observe, decide (act), reward cycle:
/// | # | O/E | I/O |            Variable 1             |           Variable 2            |      Trigger       |     MLSC     |
/// |---|-----|-----|-----------------------------------|---------------------------------|--------------------|--------------|
/// | 1 | O   | O   | MLMODEL: String                   | EXMODE: String                  | OPMODE == Name     |              |
/// | 2 | O   | I   | MLSC: String                      |                                 | EXST == STARTING   | EXECUTE      |
/// | 3 | E   | O   | MLOBSERV: Float[1..*] \{ordered\} | MLENACT: Bool[1..*] \{ordered\} | EXST == EXECUTE    |              |
/// | 4 | E   | I   | MLDECIDE: Float[1..*] \{ordered\} | MLSC: String                    | EXST == SUSPENDED  | UNSUSPENDING |
/// | 5 | E   | O   | MLREWARD: Float                   | MLREWINC: Bool                  | EXST == SUSPENDING |              |
/// | 6 | O   | I   | MLSC: String                      |                                 | EXST == COMPLETING | COMPLETED    |
/// | 7 | O   | I   | MLSTATS: String                   |                                 | MLSTATS == NULL    |              |
/// TODO MLENACT: Array -> Matrix
/// TODO MLREWINC: Not necessary --> always use set?
/// </remarks>

namespace ControlComponents.ML
{
    public interface IMLControlComponent : IControlComponent
    {
        /* Control Component: can be read directly from cc */
        // TODO define IControlComponentState and add one variable (cc) for it?
        //OperationMode OPMODE { get; }
        //ExecutionMode EXMODE { get; }
        //ExecutionState EXST { get; }

        /* ML Specific: */

        // TODO getter and setter modifier
        // use set in another interface for the operation mode
        ExecutionState MLSC { get; set; }
        string MLMODEL { get; }
        float[] MLOBSERVE { get; set; }
        bool[][] MLENACT { get; set; }

        // MLDECIDE contains the probabilities of all possible actions per action branch
        float[][] MLDECIDE { get; set; }
        float MLREWARD { get; set; }
        string MLSTATS { get; set; }

        MLProperties MLProperties { get; }
    }
}