using System;

using System.Net;
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;

public class BandwidthMonitor
{
    public DataPlanStatus dataPlan;

    public BandwidthMonitor()
    {
        ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
        
        GetDataPlanStatusInfo(InternetConnectionProfile.GetDataPlanStatus());
    }

   
    //
    //Get Connection Profile name and cost information
    //
    public string GetConnectionProfile(ConnectionProfile connectionProfile)
    {
        string connectionProfileInfo = string.Empty;
        if (connectionProfile != null)
        {
            connectionProfileInfo = "Profile Name : " + connectionProfile.ProfileName + "\n";

            //Get Dataplan Status information
            DataPlanStatus dataPlanStatus = connectionProfile.GetDataPlanStatus();
            connectionProfileInfo += GetDataPlanStatusInfo(dataPlanStatus);
            
       }

        return connectionProfileInfo;
    }//end of method  

    //
    //Display Cost based suggestions to the user
    //
    

    //
    //Display Profile Dataplan Status information
    //
   public string GetDataPlanStatusInfo(DataPlanStatus dataPlan)
    {
        string dataplanStatusInfo = string.Empty;
        dataplanStatusInfo = "Dataplan Status Information:\n";
        dataplanStatusInfo += "====================\n";

        if (dataPlan == null)
        {
            dataplanStatusInfo += "Dataplan Status not available\n";
            return dataplanStatusInfo;
        }

        if (dataPlan.DataPlanUsage != null)
        {
            dataplanStatusInfo += "Usage In Megabytes : " + dataPlan.DataPlanUsage.MegabytesUsed + "\n";
            dataplanStatusInfo += "Last Sync Time : " + dataPlan.DataPlanUsage.LastSyncTime + "\n";
        }
        else
        {
            dataplanStatusInfo += "Usage In Megabytes : Not Defined\n";
        }

        ulong? inboundBandwidth = dataPlan.InboundBitsPerSecond;
        if (inboundBandwidth.HasValue)
        {
            dataplanStatusInfo += "InboundBitsPerSecond : " + inboundBandwidth + "\n";
        }
        else
        {
            dataplanStatusInfo += "InboundBitsPerSecond : Not Defined\n";
        }

        ulong? outboundBandwidth = dataPlan.OutboundBitsPerSecond;
        if (outboundBandwidth.HasValue)
        {
            dataplanStatusInfo += "OutboundBitsPerSecond : " + outboundBandwidth + "\n";
        }
        else
        {
            dataplanStatusInfo += "OutboundBitsPerSecond : Not Defined\n";
        }

       
        return dataplanStatusInfo;
    }

    //
    //Get Network Security Settings information
    //
 
    
  
}