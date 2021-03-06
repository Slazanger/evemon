using System.Collections.Generic;
using System.IO;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models.Collections
{
    public sealed class KillLogCollection : ReadonlyCollection<KillLog>
    {
        private readonly CCPCharacter m_ccpCharacter;


        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        internal KillLogCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }

        #endregion


        #region Importation

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable kill log from the API.</param>
        internal void Import(IEnumerable<SerializableKillLogListItem> src)
        {
            Items.Clear();

            // Import the kill log from the API
            foreach (SerializableKillLogListItem srcKillLog in src)
            {
                Items.Add(new KillLog(m_ccpCharacter, srcKillLog));
            }
        }

        /// <summary>
        /// Imports the kill logs from a cached file.
        /// </summary>
        public void ImportFromCacheFile()
        {
            string filename = LocalXmlCache.GetFileInfo($"{m_ccpCharacter.Name}-{ESIAPICharacterMethods.KillLog}").FullName;

            // Abort if the file hasn't been obtained for any reason
            if (!File.Exists(filename))
                return;

            CCPAPIResult<SerializableAPIKillLog> result = Util.DeserializeAPIResultFromFile<SerializableAPIKillLog>(
                filename, APIProvider.RowsetsTransform);

            // In case the file has an error we prevent the deserialization
            if (result.HasError)
                return;

            Import(result.Result.Kills);
        }

        #endregion
    }
}