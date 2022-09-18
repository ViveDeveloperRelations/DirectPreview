using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Ping = System.Net.NetworkInformation.Ping;

public class DirectPreviewHelper
{
    public static bool PingHost(string nameOrAddress)
    {
        bool pingable = false;
        try
        {
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

        }
        catch
        {
        } //usually an invalid address will cause an exception in dispose

        return pingable;
    }
}
