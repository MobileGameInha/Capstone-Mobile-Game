using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    private Quaternion ARROW_ROTATION_LU = Quaternion.Euler(0, 0, 315);
    private Quaternion ARROW_ROTATION_LD = Quaternion.Euler(0, 0, 45);
    private Quaternion ARROW_ROTATION_RU = Quaternion.Euler(0, 0, 225);
    private Quaternion ARROW_ROTATION_RD = Quaternion.Euler(0, 0, 135);
    private Quaternion ARROW_ROTATION_HIDE = Quaternion.Euler(0, 0, 0);

    public Sprite[] tile_sprites_ = new Sprite[4];
    public Sprite tile_hide_sprite;
    public Sprite arrow_sprite;
    public Sprite hidemark_sprite;

    [SerializeField]
    private Image[] tile_images_ = new Image[GameManager.MAX_TILE_SIZE_]; //타일 이미지 : 인스펙터에서 지정
    private Image[] arrows_ = new Image[GameManager.MAX_TILE_SIZE_]; //각 타일의 화살표



    private void Awake()
    {
        for (int  i = 0;  i < GameManager.MAX_TILE_SIZE_;  i++)
        {
            arrows_[i] = tile_images_[i].GetComponentsInChildren<Image>()[1];
        }
    }

    public void SetState(bool is_active, int idx, ArrowDirection dir=ArrowDirection.LU, bool hide = false) {
        if (is_active)
        {
            if (hide && idx!=0)
            {
                tile_images_[idx].gameObject.SetActive(true);
                tile_images_[idx].sprite = tile_hide_sprite;
                arrows_[idx].sprite = hidemark_sprite;
                arrows_[idx].transform.rotation = ARROW_ROTATION_HIDE;
            }
            else 
            {
                tile_images_[idx].gameObject.SetActive(true);
                tile_images_[idx].sprite = tile_sprites_[((int)dir)];
                arrows_[idx].sprite = arrow_sprite;
                switch (dir)
                {
                    case ArrowDirection.LU:
                        arrows_[idx].transform.rotation = ARROW_ROTATION_LU;
                        break;
                    case ArrowDirection.LD:
                        arrows_[idx].transform.rotation = ARROW_ROTATION_LD;
                        break;
                    case ArrowDirection.RU:
                        arrows_[idx].transform.rotation = ARROW_ROTATION_RU;
                        break;
                    case ArrowDirection.RD:
                        arrows_[idx].transform.rotation = ARROW_ROTATION_RD;
                        break;
                }
            }
        }
        else {
            tile_images_[idx].gameObject.SetActive(false);
            //++성공 유무에 따른 이펙트 변경 예정
        }
    }//각 타일 상태(보이는지, 화살표가 어디인지) 지정
    


}
