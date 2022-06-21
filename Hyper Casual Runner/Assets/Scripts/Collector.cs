using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collector : MonoBehaviour
{
    public GameObject mainCube;
    public GameObject mainTrail;
    float height;
    bool waiting=false;
    bool obstacleStack = false;
    bool speedBoostActive = true;
    bool disableTrail = false;
    bool notEffected = false;
    int sibIndex = 3;
    int howManyDestroys = 0;
    int collectDestroys = 0;
    int berry, crystal, diamond;
    List<int> positions = new List<int>();

    void Start()
    {
        mainCube = GameObject.Find("Character");
        mainTrail = GameObject.Find("Trail");
    }

    void Update()
    {
        mainCube.transform.position = new Vector3(transform.position.x, height, transform.position.z);
        this.transform.localPosition = new Vector3(0, -height, 0);
        if (mainCube.transform.GetChild(mainCube.transform.childCount - 1).gameObject.GetComponent<Renderer>().material == mainCube.transform.GetChild(2).gameObject.GetComponent<Renderer>().material || disableTrail==true)
        {
            mainTrail.GetComponent<Renderer>().enabled = false; //we make the trail invisible if we have no cube under the character or if we are in the air(by using jump pad)
        }
        else
        {
            mainTrail.GetComponent<Renderer>().enabled = true;
        }
        mainTrail.GetComponent<Renderer>().material = mainCube.transform.GetChild(mainCube.transform.childCount - 1).gameObject.GetComponent<Renderer>().material;
        //we set the material of our trail to be the last cube we collected
        if (speedBoostActive == false)
        {
            mainCube.transform.GetComponent<PlayerController>().backToRunningSpeed();//when speed boost ends, we go back to normal speed
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mainCube.transform.childCount == 3) //it means that no cube is attached to the character
        {
            NoCubeAttached(other);
        }
        if (other.gameObject.tag=="Collect")
        {
            CollectObject(other);
        }
        else if (other.gameObject.tag == "BigWall" && obstacleStack==false)
            //we check if obstacleStack is false because even if we hit big wall and small wall at the same time, we should not destroy 3, we should only destroy 2
        {
            BigWallEffect();
        }
        else if (other.gameObject.tag == "SmallWall" && obstacleStack==false)
        {
            SmallWallEffect();
        }
        else if (other.gameObject.tag == "OrderGate")
        {
            OrderGateEffect();
        }
        else if(other.gameObject.tag == "Ordering")
            //I placed an ordering object just at the end of order gate because Object.Destroy is delayed until after the current update loop.
            //So if I tried to set positions of cubes according to their child index in order gate, I got the wrong result because the destroying process wasn't done yet.
        {
            OrderingEffect();
        }

        else if(other.gameObject.tag == "SpeedBoost")//we get speed boost and gain immunity to obstacles if we cross over speed boost
        {
            speedBoostActive = true;
            mainCube.transform.GetComponent<PlayerController>().changeRunningSpeed(15);
            notEffected = true;
            StartCoroutine(SpeedBoostCoroutine());
        }
        else if(other.gameObject.tag == "JumpPad")//character height gets altered for some time in a coroutine and then gets back to normal
        {
            //StartCoroutine(JumpCoroutine());
        }
        else if (other.gameObject.tag == "DestroyRandom")
        {
            cubeDestroyer();
        }
        else if (other.gameObject.tag == "RandomGate")
        {
            RandomGateEffect();
        }
        else if(other.gameObject.tag == "FinishLine")
        {
            if(SceneManager.GetActiveScene().buildIndex==1)
            {
                Application.Quit();
                //UnityEditor.EditorApplication.isPlaying = false;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    private void RandomGateEffect()
    {
        for (int allChilds = 3; allChilds < mainCube.transform.childCount; allChilds++)
        {
            positions.Add(allChilds);
        }
        for (int i = 3; i < mainCube.transform.childCount; i++)
        {
            int _index = UnityEngine.Random.Range(0, positions.Count - 1);
            mainCube.transform.GetChild(i).transform.SetSiblingIndex(positions[_index]);
            positions.Remove(_index);
        }
    }

    private void OrderingEffect()
    {
        for (int allChilds = 3; allChilds < mainCube.transform.childCount; allChilds++)
        {
            if (mainCube.transform.GetChild(allChilds).GetComponent<BerryScript>())
            {
                mainCube.transform.GetChild(allChilds).GetComponent<BerryScript>().setIndex(allChilds);
            }
            else if (mainCube.transform.GetChild(allChilds).GetComponent<CrystalScript>())
            {
                mainCube.transform.GetChild(allChilds).GetComponent<CrystalScript>().setIndex(allChilds);
            }
            else if (mainCube.transform.GetChild(allChilds).GetComponent<DiamondScript>())
            {
                mainCube.transform.GetChild(allChilds).GetComponent<DiamondScript>().setIndex(allChilds);
            }
        }
        if (howManyDestroys == 3)//Get speed boost and not be affected from targets if we match 3 in a row
        {
            speedBoostActive = true;
            mainCube.transform.GetComponent<PlayerController>().changeRunningSpeed(15);
            notEffected = true;
            StartCoroutine(SpeedBoostCoroutine());
        }
        howManyDestroys = 0;
    }

    private void OrderGateEffect()
    {
        sibIndex = 3;
        berry = 0;
        crystal = 0;
        diamond = 0;
        for (int j = 3; j < mainCube.transform.childCount; j++)
        {
            if (mainCube.transform.GetChild(j).GetComponent<BerryScript>())//we set the berry colored cubes to be in first indexes as a children.
            {
                berry++;
                mainCube.transform.GetChild(j).transform.SetSiblingIndex(sibIndex);
                int counter = 0;
                if (berry == 3)//when we get through order gate, we'll always have less than 3 objects of same color. If we get to 3 then destroy
                {
                    for (int allChilds = 3; allChilds < mainCube.transform.childCount; allChilds++)
                    {
                        if (mainCube.transform.GetChild(allChilds).GetComponent<BerryScript>() && counter < 3)
                        {
                            Destroy(mainCube.transform.GetChild(allChilds).gameObject);
                            counter++;
                        }
                    }
                    howManyDestroys++;
                    height -= 3;
                    berry = 0;
                }
                sibIndex++;
            }
        }
        for (int k = 3; k < mainCube.transform.childCount; k++)
        {
            if (mainCube.transform.GetChild(k).GetComponent<CrystalScript>())
            {
                crystal++;
                mainCube.transform.GetChild(k).transform.SetSiblingIndex(sibIndex);//we set the crystal colored cubes to come after berry colored ones as children.
                int counter = 0;
                if (crystal == 3)
                {
                    for (int allChilds = 3; allChilds < mainCube.transform.childCount; allChilds++)
                    {
                        if (mainCube.transform.GetChild(allChilds).GetComponent<CrystalScript>() && counter < 3)
                        {
                            Destroy(mainCube.transform.GetChild(allChilds).gameObject);
                            counter++;
                        }
                    }
                    howManyDestroys++;
                    height -= 3;
                    crystal = 0;
                }
                sibIndex++;
            }
        }
        for (int l = 3; l < mainCube.transform.childCount; l++)
        {
            if (mainCube.transform.GetChild(l).GetComponent<DiamondScript>())
            {
                diamond++;
                mainCube.transform.GetChild(l).transform.SetSiblingIndex(sibIndex);//we set the diamond colored cubes to come after crystal colored ones as children.
                int counter = 0;
                if (diamond == 3)
                {
                    for (int allChilds = 3; allChilds < mainCube.transform.childCount; allChilds++)
                    {
                        if (mainCube.transform.GetChild(allChilds).GetComponent<DiamondScript>() && counter < 3)
                        {
                            Destroy(mainCube.transform.GetChild(allChilds).gameObject);
                            counter++;
                        }
                    }
                    howManyDestroys++;
                    height -= 3;
                    diamond = 0;
                }
                sibIndex++;
            }
        }
    }

    private void SmallWallEffect()
    {
        if (notEffected == false)
        {
            Destroy(mainCube.transform.GetChild(mainCube.transform.childCount - 1).gameObject);
            height -= 1;
            obstacleStack = true;
            StartCoroutine(ObstacleWaitingCoroutine());
        }
        else//speed boost is active so we're not affected
        {
        }
    }

    private void BigWallEffect()
    {
        if (notEffected == false)
        {
            if (mainCube.transform.childCount == 4)//If we hit the big wall when we have only 1 cube, we fail the level
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                /*Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false;*/
            }
            else//If we hit the big wall when we have more than 1 cube
            {
                Destroy(mainCube.transform.GetChild(mainCube.transform.childCount - 1).gameObject);
                Destroy(mainCube.transform.GetChild(mainCube.transform.childCount - 2).gameObject);
                height -= 2;
                obstacleStack = true;
            }
        }
        else//speed boost is active so we're not affected
        {
        }
    }

    private void CollectObject(Collider other)
    {
        height += 1;
        other.gameObject.GetComponent<CollectibleCube>().itIsCollected();
        other.gameObject.transform.parent = mainCube.transform;
        other.gameObject.GetComponent<CollectibleCube>().setIndex(height);
        other.gameObject.GetComponent<CollectibleCube>().setCollected();
        cubeDestroyer();
        if (collectDestroys == 3)//fever mode if we match 3 in a row
        {
            speedBoostActive = true;
            mainCube.transform.GetComponent<PlayerController>().changeRunningSpeed(15);
            notEffected = true;
            StartCoroutine(SpeedBoostCoroutine());
            collectDestroys = 0;
        }
    }

    private void NoCubeAttached(Collider other)
    {
        if (notEffected == true)//speed boost is active so we're not affected
        {
        }
        else if (other.tag == "Fire")//if we hit fire with no cubes,restart level
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (other.tag == "SmallWall")//if we hit the small wall with no cubes, restart level
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (other.tag == "BigWall")//if we hit the big wall with no cubes, restart level
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Fire" && notEffected==true)
        {

        }
        else if(waiting == false && other.tag == "Fire")
        {
            if (mainCube.transform.childCount > 3)
            {
                Destroy(mainCube.transform.GetChild(mainCube.transform.childCount - 1).gameObject);
                waiting = true;
                height -= 1;
                StartCoroutine(WaitingCoroutine());
            }
            else if(mainCube.transform.childCount==3)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//we restart the level
            }
        }
    }

    void cubeDestroyer()
    {
        for(int i=3;i<mainCube.transform.childCount-2;i++)
        {
            if(mainCube.transform.GetChild(i).GetComponent<BerryScript>())
            {
                if(mainCube.transform.GetChild(i+1).GetComponent<BerryScript>())
                {
                    if (mainCube.transform.GetChild(i+2).GetComponent<BerryScript>())
                    {
                        Debug.Log("berry destroyed");
                        height -= 3;
                        Destroy(mainCube.transform.GetChild(i).gameObject);
                        Destroy(mainCube.transform.GetChild(i+1).gameObject);
                        Destroy(mainCube.transform.GetChild(i+2).gameObject);
                        collectDestroys++;
                    }
                }
            }
            else if (mainCube.transform.GetChild(i).GetComponent<CrystalScript>())
            {
                if (mainCube.transform.GetChild(i + 1).GetComponent<CrystalScript>())
                {
                    if (mainCube.transform.GetChild(i + 2).GetComponent<CrystalScript>())
                    {
                        Debug.Log("crystal destroyed");
                        height -= 3;
                        Destroy(mainCube.transform.GetChild(i).gameObject);
                        Destroy(mainCube.transform.GetChild(i + 1).gameObject);
                        Destroy(mainCube.transform.GetChild(i + 2).gameObject);
                        collectDestroys++;
                    }
                }
            }
            else if (mainCube.transform.GetChild(i).GetComponent<DiamondScript>())
            {
                if (mainCube.transform.GetChild(i + 1).GetComponent<DiamondScript>())
                {
                    if (mainCube.transform.GetChild(i + 2).GetComponent<DiamondScript>())
                    {
                        Debug.Log("diamond destroyed");
                        height -= 3;
                        Destroy(mainCube.transform.GetChild(i).gameObject);
                        Destroy(mainCube.transform.GetChild(i + 1).gameObject);
                        Destroy(mainCube.transform.GetChild(i + 2).gameObject);
                        collectDestroys++;
                    }
                }
            }
        }
    }

    IEnumerator WaitingCoroutine()
    {
        yield return new WaitForSeconds(.25f);
        waiting = false;
    }

    IEnumerator ObstacleWaitingCoroutine()
    {
        yield return new WaitForSeconds(1);
        obstacleStack = false;
    }

    IEnumerator SpeedBoostCoroutine()
    {
        yield return new WaitForSeconds(3);
        speedBoostActive = false;
        notEffected = false;
    }
    /*IEnumerator JumpCoroutine()
    {
        disableTrail = true;
        for (int i=0;i<35;i++)
        {
            height += 0.12f;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 35; i++)
        {
            height += 0.08f;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 35; i++)
        {
            height -= 0.08f;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 35; i++)
        {
            height -= 0.12f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(.2f);
        disableTrail = false;
    }*/

}