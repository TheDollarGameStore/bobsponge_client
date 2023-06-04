using System;
using System.Collections.Generic;

[Serializable]
public class StoryModel
{
    public string _id;
    public string requestor_id;
    public string topic;
    public List<Scenario> scenario;
}

[Serializable]
public class Scenario
{
    public string character;
    public string text;
    public string sound;
}