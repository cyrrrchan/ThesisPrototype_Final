using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using FullSerializer;

//I put this Watson code in my PlayerController file because it makes the most sense for my use case. 
public class WatsonConversation : MonoBehaviour
{
    [SerializeField]
    private fsSerializer _serializer = new fsSerializer();

    void Start()
    {
        //enter username and password as a string
        Credentials credentials = new Credentials("a0a60346-5e86-4b23-8880-4659e0d1a2c4", //username
            "noTPQG1ZOgM6", //password
            "https://gateway.watsonplatform.net/conversation/api"); //url
        Conversation _conversation = new Conversation(credentials);

        //be sure to give it a Version Date
        _conversation.VersionDate = "2017-05-26";

        //enter workspace_id as string
        if (!_conversation.Message(OnMessage, OnFail, "Watson Assistant Tutorial", "I'd like a pizza!"))
        {
            Log.Debug("ExampleConversation.Message()", "Failed to message!");
        }
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleConversation.OnFail()", "Error received: {0}", error.ToString());
    }

    private void OnMessage(object resp, Dictionary<string, object> customData)
    {
        fsData fsdata = null;
        fsResult r = _serializer.TrySerialize(resp.GetType(), resp, out fsdata);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsdata to MessageResponse
        MessageResponse messageResponse = new MessageResponse();
        object obj = messageResponse;
        r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //if we get a response, do something with it (find the intents, output text, etc.)
        if (resp != null && (messageResponse.intents.Length > 0 || messageResponse.entities.Length > 0))
        {
            string intent = messageResponse.intents[0].intent;
            Debug.Log("Intent: " + intent);
            string outputText = messageResponse.output.text[0];
            Debug.Log("Output Text: " + outputText);
        }

        //whole object
        Debug.Log(customData["json"].ToString());
    }
}