using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopoSunbatheQuest : TalkableCharacter
{
    public NonPlayableCharacter touristToAnnoy;
    public float distance = 2f;
    public GameObject rewardHelmet;

    bool isFinished = false;
    bool firstTimeTalking = true;

    public override Dialog OnDialogTrigger()
    {
        var lib = Game.i.library.dialogs;
        if (firstTimeTalking) {
            firstTimeTalking = false;
            return lib["sunbathe_beginning"];
        }
        if (isFinished) {
            return lib["sunbathe_end"];
        }
        else {
            return lib["sunbathe_noResolution"];
        }
    }

    new private void Update()
    {
        base.Update();

        if (isFinished) return;

        var controller = Game.i.player.GetCurrentController();
        if (controller is Umbrella) {
            if (
                controller.AreLegsRetracted()
                && Vector3.Distance(controller.transform.position, touristToAnnoy.transform.position) < distance
                && touristToAnnoy.activity is Serviette
            ) {
                Pyromancer.PlayGameEffect("gfx_bottle_flip_success", controller.transform.position);
                touristToAnnoy.activity.Exit(touristToAnnoy);
                touristToAnnoy.emo.Show(Emotion.EVerb.HATE, "sunshade");
                touristToAnnoy.graph.Move(6406); // Goes back to Venturing state
                isFinished = true;
                AddHat();
            }
        }
    }


    void AddHat()
    {
        GameObject _mouette = Instantiate(rewardHelmet, Game.i.player.baseController.visuals.transform);
        _mouette.transform.localPosition = new Vector3(0, .2f, 0);
        _mouette.transform.localEulerAngles = new Vector3(90, 180, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Load(byte[] data)
    {
        throw new System.NotImplementedException();
    }
    public override byte[] Save()
    {
        throw new System.NotImplementedException();
    }
}
