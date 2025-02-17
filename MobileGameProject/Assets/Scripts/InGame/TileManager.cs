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

    public Sprite[] tile_sprites_ = new Sprite[4];

    [SerializeField]
    private Image[] tile_images_ = new Image[GameManager.MAX_TILE_SIZE_]; //Ÿ�� �̹��� : �ν����Ϳ��� ����
    private Transform[] arrows_ = new Transform[GameManager.MAX_TILE_SIZE_]; //�� Ÿ���� ȭ��ǥ


    private void Awake()
    {
        for (int  i = 0;  i < GameManager.MAX_TILE_SIZE_;  i++)
        {
            arrows_[i] = tile_images_[i].GetComponentsInChildren<Transform>()[1];
        }
    }

    public void SetState(bool is_active, int idx, ArrowDirection dir=ArrowDirection.LU) {
        if (is_active)
        {
            tile_images_[idx].gameObject.SetActive(true);
            tile_images_[idx].sprite = tile_sprites_[((int)dir)];
            switch (dir)
            {
                case ArrowDirection.LU:
                    arrows_[idx].rotation = ARROW_ROTATION_LU;
                    break;
                case ArrowDirection.LD:
                    arrows_[idx].rotation = ARROW_ROTATION_LD;
                    break;
                case ArrowDirection.RU:
                    arrows_[idx].rotation = ARROW_ROTATION_RU;
                    break;
                case ArrowDirection.RD:
                    arrows_[idx].rotation = ARROW_ROTATION_RD;
                    break;
            }
        }
        else {
            tile_images_[idx].gameObject.SetActive(false);
            //++���� ������ ���� ����Ʈ ���� ����
        }
    }//�� Ÿ�� ����(���̴���, ȭ��ǥ�� �������) ����
    


}
