using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace DeepPilotCsharp
{
    /// <summary>
    /// Represent a message that the Raspberry send to the PC.
    /// </summary>
    class MessageFromRaspberryToComputer
    {
        /// <summary>
        /// This is the only object : value pair which is sent by default. The time starts when the Raspberry python script is started.
        /// </summary>
        public Single? Time { get; set; }

        /// <summary>
        /// The race time starts when an actual race starts.
        /// </summary>
        public Single? TimeRaceS { get; set; }

        /// <summary>
        /// Actual number of lap.
        /// </summary>
        public Int32? LapNumber { get; set; }

        /// <summary>
        /// The actual distance the vehicle has taken.
        /// </summary>
        public Single? Distance { get; set; }

        /// <summary>
        /// Only the race distance.
        /// </summary>
        public Single? DistanceRaceM { get; set; }

        /// <summary>
        /// The speed according to dynamixel standards.
        /// </summary>
        public Single? VehicleSpeedDyna { get; set; }

        /// <summary>
        /// The speed in km/h.
        /// </summary>
        public Single? VehicleSpeedKmph { get; set; }

        /// <summary>
        /// The speed in m/s.
        /// </summary>
        public Single? VehicleSpeedMps { get; set; }

        /// <summary>
        /// Electric power of the motor.
        /// </summary>
        public Single? PowerMotorW { get; set; }

        /// <summary>
        /// Electric energy of the motor in Joule.
        /// </summary>
        public Single? EnergyMotorJ { get; set; }

        /// <summary>
        /// Actual position of the wheel (drive) motor.
        /// </summary>
        public Int32? MotorPositionDrive { get; set; }

        /// <summary>
        /// Actual load of the wheel (drive) motor.
        /// </summary>
        public Int32? MotorLoadDrive { get; set; }

        /// <summary>
        /// The X axes acceleration.
        /// </summary>
        public Single? AccXG { get; set; }

        /// <summary>
        /// The Y axes acceleration.
        /// </summary>
        public Single? AccYG { get; set; }

        /// <summary>
        /// The Z axes acceleration.
        /// </summary>
        public Single? AccZG { get; set; }

        /// <summary>
        /// Create a JSON string from the current object.
        /// </summary>
        /// <returns>JSON string that represent the object.</returns>
        public string Serialize()
        {
            return new JavaScriptSerializer().Serialize(this);
        }

        /// <summary>
        /// Create a message object from a correct JSON string.
        /// </summary>
        /// <param name="json">The JSON string what you want to deserialize.</param>
        /// <returns>A deserialized message object.</returns>
        public static MessageFromRaspberryToComputer Deserialize(string json)
        {
            return new JavaScriptSerializer().Deserialize<MessageFromRaspberryToComputer>(json);
        }

        /// <summary>
        /// Simple ToString method.
        /// </summary>
        /// <returns>A string that contains all the values.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Time: " + Time);
            sb.AppendLine("TimeRaceS: " + TimeRaceS);
            sb.AppendLine("LapNumber: " + LapNumber);
            sb.AppendLine("Distance: " + Distance);
            sb.AppendLine("DistanceRaceM: " + DistanceRaceM);
            sb.AppendLine("VehicleSpeedDyna: " + VehicleSpeedDyna);
            sb.AppendLine("VehicleSpeedKmph: " + VehicleSpeedKmph);
            sb.AppendLine("VehicleSpeedMps: " + VehicleSpeedMps);
            sb.AppendLine("PowerMotorW: " + PowerMotorW);
            sb.AppendLine("EnergyMotorJ: " + EnergyMotorJ);
            sb.AppendLine("MotorPositionDrive: " + MotorPositionDrive);
            sb.AppendLine("MotorLoadDrive: " + MotorLoadDrive);
            sb.AppendLine("AccXG: " + AccXG);
            sb.AppendLine("AccYG: " + AccYG);
            sb.AppendLine("AccZG: " + AccZG);

            return sb.ToString();
        }

    }
}
