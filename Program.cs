﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatHub 
{
    class Program
    {
	public static DeviceUplinkBuffer[] deviceUplinkBuffers;
	public static DeviceDownlinkBuffer[] deviceDownlinkBuffers;
	public static Memory[] memories;
	public SendEvent[] sendEvents;
	public RequestEvent[] requestEvents;
	public static CsvObj[][] csvObjs;
	public static SatelliteUplinkBuffer satelliteUplinkBuffer;
	public static SatelliteDownlinkBuffer satelliteDownlinkBuffer;


        static void Main(string[] args)
        {
	    deviceUplinkBuffers = new DeviceUplinkBuffer[3];
	    deviceDownlinkBuffers = new DeviceDownlinkBuffer[3];
            memories = new Memory[16];
	    satelliteUplinkBuffer = new SatelliteUplinkBuffer(1333344, 100);            
	    satelliteDownlinkBuffer = new SatelliteDownlinkBuffer(1333344, 150);            
	    sendEvents = new SendEvent[40];
	    requestEvents = new RequestEvent[40];
	    int tCurrentClock = 0;

            for(int i = 0; i < 2; i++)
            {
                memories[i] = new Memory(256, 1);
            }
            for(int i = 2; i < 6; i++)
            {
                memories[i] = new Memory(512, 8);
            }
            for(int i = 6; i < 16; i++)
            {
                memories[i] = new Memory(1024, 15);
            }
	    	    
	    csvObjs = new CsvObj[3][15];
		csvObjs[0][0] = new CsvObj(0,1,"SEND",128,1);
		csvObjs[0][1] = new CsvObj(150,1,"SEND",1024,4);
		csvObjs[0][2] = new CsvObj(210,1,"REQUEST",128,2);
		csvObjs[0][3] = new CsvObj(1000,1,"REQUEST",128,3);
		csvObjs[0][4] = new CsvObj(12000,1,"REQUEST",1024,10);
		csvObjs[1][0] = new CsvObj(50,2,"SEND",128,2);
		csvObjs[1][1] = new CsvObj(200,2,"REQUEST",128,1);
		csvObjs[1][2] = new CsvObj(1100,2,"REQUEST",128,3);
		csvObjs[1][3] = new CsvObj(11500,2,"REQUEST",1024,12);
		csvObjs[2][0] = new CsvObj(100,3,"SEND",128,3);
		csvObjs[2][1] = new CsvObj(300,3,"SEND",1024,5);
		csvObjs[2][2] = new CsvObj(400,3,"SEND",1024,6);
		csvObjs[2][3] = new CsvObj(500,3,"SEND",512,7);
		csvObjs[2][4] = new CsvObj(2000,3,"SEND",1024,8);
		csvObjs[2][5] = new CsvObj(3000,3,"SEND",1024,9);
		csvObjs[2][6] = new CsvObj(4000,3,"SEND",1024,10);
		csvObjs[2][7] = new CsvObj(5000,3,"SEND",1024,11);
		csvObjs[2][8] = new CsvObj(6000,3,"SEND",1024,12);
		csvObjs[2][9] = new CsvObj(7000,3,"SEND",1024,13);
		csvObjs[2][10] = new CsvObj(8000,3,"SEND",1024,14);
		csvObjs[2][11] = new CsvObj(9000,3,"SEND",1024,15);
		csvObjs[2][12] = new CsvObj(10000,3,"SEND",1024,16);
		csvObjs[2][13] = new CsvObj(11000,3,"SEND",1024,17);
		csvObjs[2][14] = new CsvObj(12100,3,"REQUEST",512,7);
	    latencies = new int[3][15];
	    csvTracker = new int[3];
	    int ii;
	    for (ii = 0; ii < 3; ii++)
	    {
		csvTracker[ii]=0;
	    }	    
            while (tCurrentClock < 50000000)
            {
		int i;
		int count = 0;
		for (i = 0;i < 3; i++)
		{
		    if (sendEvents[i] > -1)
		    {
			count++;
		    }
		}
		int numSendEvents;
		numSendEvents = count;

		count = 0;
		for (i = 0;i < 3; i++)
		{
		    if (requestEvents[i] > -1)
		    {
			count++;
		    }
		}
		int numRequestEvents;
		numRequestEvents = count;
		

		int finished;	
		for(i = 0; i < numSendEvents; i++)
                {
		    if (SendEvents[i]._evicting == true)
		    {
                    	finished = SendEvents[i].updateSendEvent(tCurrentClock, deviceUplinkBuffer[i], memories[SendEvents[i]._memoryDesignation], satelliteUplinkBuffer);
		    }
		    else
		    {
			finished = SendEvents[i].updateSendEvent(tCurrentClock, deviceUplinkBuffer[i], memories[SendEvents[i]._memoryDesignation]);
		    }
		    if (finished == 1)
		    {
			    latencies[i][csvTracker[i]] = SendEvents[i]._finishTime - csvObjs[i][csvTracker[i]]._csvTime;
			    csvTracker[i] += 1;
			    if (csvObjs[csvTracker[i]]._operation == "SEND")
			    {
				int j;
				j = whichMemoryToSend(csvObjs[i][csvTracker[i]]._size, memories);
				if (j == -1)
				{
				    j = whichMemoryToEvict(csvObjs[i][csvTracker[i]]._size);
				    SendEvents[i] = new SendEvent(csvObjs[i][csvTracker[i]]._csvTime, csvObjs[i][csvTracker[i]]._size, csvObjs[i][csvTracker[i]]._trDataTag, i, true, j);	
				}
				else
				{
				    SendEvents[i] = new SendEvent(csvObjs[i][csvTracker[i]]._csvTime, csvObjs[i][csvTracker[i]]._size, csvObjs[i][csvTracker[i]]._trDataTag, i, false, j);
				}
			    }
			    else
			    {
				csvTracker[i] += 1;
				int m;
				m = checkTag(csvObjs[i][csvTracker[i]]._trDataTag, memories);
				if (m == -1)
				{
				    RequestEvent[i] = new RequestEvent(csvObjs[i][csvTracker[i]]._csvTime, csvObjs[i][csvTracker[i]]._size, csvObjs[i][csvTracker[i]]._trDataTag, false, -1);
				}
				else
				{
				    RequestEvent[i] = new RequestEvent(csvObjs[i][csvTracker[i]]._csvTime, csvObjs[i][csvTracker[i]]._size, csvObjs[i][csvTracker[i]]._trDataTag, true, m);
				}				
			    }
		    } 
                }
                
		for(i = 0; i < numRequestEvents; i++)
                {
                    if (RequestEvents[i]._inMemory == 1)
		    {
			finished = RequestEvents[i].updateRequestEvent(tCurrentClock, deviceDownlinkBuffer[i], memories[RequestEvents[i]._memoryDesignation]);
		    }
		    else
		    {
			finished = RequestEvents[i].updateRequestEvent(tCurrentClock, deviceDownlinkBuffer[i], satelliteUplinkBuffer, satelliteDownlinkBuffer);
		    }
		    if (finished == 1)
		    {
			latencies[i][csvTracker[i]] = SendEvents[i]._finishTime - csvObjs[i][csvTracker[i]]._csvTime;
		    }
		}
                tCurrentClock++;
            }
            
        }

	private int checkTags (int tag, Memory[] memory)
	{
	    int i;
	    for (i = 0;i < 16; i++)
	    {
		if (memories[i]._trDataTag == tag)
		{
		    return i;
		}
	    }
	    return -1;
	}

	private int whichMemoryToSend (int size, Memory[] memory)
	{
	    int i;
	    if (size==128)
	    {
		for (i = 0;i < 2;i++)
		{
		    if (memories[i]._trDataTag == 0)
		    {
			return i;
		    }
		}
		return -1;
	    }	
	    if (size==512)
	    {
		for (i = 2;i < 6;i++)
		{
		    if (memories[i]._trDataTag == 0)
		    {
			return i;
		    }
		}
		return -1;
	    }	
	    if (size==1024)
	    {
		for (i = 6;i < 16;i++)
		{
		    if (memories[i]._trDataTag == 0)
		    {
			return i;
		    }
		}
		return -1;
	    }	
	}
	
	private int whichMemoryToEvict (int size)
	{
	    if (size==128)
	    {
		return 0;
	    }	
	    if (size==512)
	    {
		return 2;
	    }	
	    if (size==1024)
	    {
		return 6;
	    }	
	    
	}
    }
}