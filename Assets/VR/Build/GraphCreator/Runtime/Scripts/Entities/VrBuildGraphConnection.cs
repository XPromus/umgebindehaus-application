using System;
using UnityEngine.Serialization;

namespace VR.Build.GraphCreator.Runtime.Scripts.Entities
{
    [Serializable]
    public struct VrBuildGraphConnection
    {
        public VrBuildGraphConnectionPort inputPort;
        public VrBuildGraphConnectionPort outputPort;

        public VrBuildGraphConnection(VrBuildGraphConnectionPort inputPort, VrBuildGraphConnectionPort outputPort)
        {
            this.inputPort = inputPort;
            this.outputPort = outputPort;
        }

        public VrBuildGraphConnection(string inputNodeId, int inputPortIndex, string outputNodeId, int outputPortIndex)
        {
            inputPort = new VrBuildGraphConnectionPort(inputNodeId, inputPortIndex);
            outputPort = new VrBuildGraphConnectionPort(outputNodeId, outputPortIndex);
        }
    }
    
    [Serializable]
    public struct VrBuildGraphConnectionPort
    {
        public string nodeId;
        public int portIndex;

        public VrBuildGraphConnectionPort(string nodeId, int portIndex)
        {
            this.nodeId = nodeId;
            this.portIndex = portIndex;
        }
    }
}