using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;

namespace DeepPilotCsharp
{

    /// <summary>
    /// Represent a message that the PC send to the Raspberry.
    /// </summary>
    class MessageFromComputerToRaspberry
    {

        /// <summary>
        /// If this is true the Raspberry python script will be stopped.
        /// </summary>
        public Boolean? Exit { get; set; }

        /// <summary>
        ///  {"SetDelay": 1.2} means the new frequency is 0.83 Hz
        /// </summary>
        public Single? SetDelay { get; set; }

        /// <summary>
        /// Set the IP adress where to send the messages. (Default: 192.168.0.10)
        /// </summary>
        public String SetUdpIpSend { get; set; }

        /// <summary>
        /// The drive motor load signal reference.
        /// </summary>
        public Int32? SetLoadSignalReferenceDrive { get; set; }

        /// <summary>
        /// The drive motor speed signal reference according the dynamixel standards. (0-1023 forward and 1024-2047 backwards)
        /// </summary>
        public Int32? SetSpeedSignalReferenceDrive { get; set; }

        /// <summary>
        /// Set the drive motor speed signal reference in km/h.
        /// </summary>
        public Single? SetSpeedSignalReferenceDriveKmph { get; set; }

        /// <summary>
        /// Set the drive motor speed signal reference in m/s.
        /// </summary>
        public Single? SetDpeedSignalReferenceMps { get; set; }

        /// <summary>
        /// The steering wheel angle reference.
        /// </summary>
        public Int32? SetAngleReferenceSteering { get; set; }

        /// <summary>
        /// Sending / stop sending the motor position.
        /// </summary>
        public Boolean? SendMotorPositionDrive { get; set; }

        /// <summary>
        /// Sending / stop sending the motor speed according the dynamixel standards.
        /// </summary>
        public Boolean? SendMotorSpeedDrive { get; set; }

        /// <summary>
        /// Sending / stop sending the motor speed in km/h.
        /// </summary>
        public Boolean? SendMotorSpeedDriveKmph { get; set; }

        /// <summary>
        /// Sending / stop sending the motor speed in m/s.
        /// </summary>
        public Boolean? SendMotorSpeedDriveMps { get; set; }

        /// <summary>
        /// Sending / stop sending the motor load according the dynamixel standards.
        /// </summary>
        public Boolean? SendMotorLoadDrive { get; set; }

        /// <summary>
        /// Sending / stop sending the lap number.
        /// </summary>
        public Boolean? SendLapNumber { get; set; }

        /// <summary>
        /// Sending / stop sending camera 1 feed.
        /// </summary>
        public Boolean? SendCamera1 { get; set; }

        /// <summary>
        /// Sending / stop sending camera 2 feed.
        /// </summary>
        public Boolean? SendCamera2 { get; set; }

        /// <summary>
        /// Sending / stop sending all values.
        /// </summary>
        public Boolean? SendAll { get; set; }

        /// <summary>
        /// Sending / stop sending important values.
        /// </summary>
        public Boolean? SendImportant { get; set; }

        /// <summary>
        /// Create a JSON string from the current object.
        /// </summary>
        /// <returns>JSON string that represent the object.</returns>
        public string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            //return new JavaScriptSerializer().Serialize(this);
        }

        /// <summary>
        /// Create a message object from a correct JSON string.
        /// </summary>
        /// <param name="json">The JSON string what you want to deserialize.</param>
        /// <returns>A deserialized message object.</returns>
        public static MessageFromComputerToRaspberry Deserialize(string json)
        {
            return new JavaScriptSerializer().Deserialize<MessageFromComputerToRaspberry>(json);
        }

    }
}
