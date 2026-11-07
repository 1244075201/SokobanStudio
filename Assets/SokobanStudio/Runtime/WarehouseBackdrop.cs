using UnityEngine;
using UnityEngine.UI;
namespace SokobanStudio {
// Decorative warehouse only: no gameplay data or colliders.
public class WarehouseBackdrop:MonoBehaviour {
 Texture2D texture;Sprite sprite;WarehouseArt art;
 void Awake(){art=new WarehouseArt();const int cols=24,rows=14,cell=32;texture=new Texture2D(cols*cell,rows*cell);texture.filterMode=FilterMode.Bilinear;
 for(int gy=0;gy<rows;gy++)for(int gx=0;gx<cols;gx++){
 bool center=gx>=8&&gx<=15;bool wall=!center&&((gy==2||gy==10)&&(gx<7||gx>17)||((gx==3||gx==20)&&gy>2&&gy<11));
 string kind=wall?"wall":(gx==5&&gy==5||gx==18&&gy==8)?"crate":(gx==6&&gy==5)?"robot":(gx==18&&gy==7)?"goal":"floor";
 var pixels=art.Art(kind).texture;
 for(int y=0;y<cell;y++)for(int x=0;x<cell;x++){float nx=(gx*cell+x)/(float)(cols*cell),ny=(gy*cell+y)/(float)(rows*cell);float edge=Mathf.Clamp01(Mathf.Abs(nx-.5f)*1.8f);float glow=Mathf.Exp(-((nx-.78f)*(nx-.78f)+(ny-.6f)*(ny-.6f))*10);Color baseColor=new Color(.025f,.045f,.075f);Color tile=pixels.GetPixel(x*2,y*2);float strength=center?.09f:.32f+glow*.18f;Color c=Color.Lerp(baseColor,tile,strength*tile.a);c*=1-.3f*edge;if(!center&&(gx==1||gx==22)&&y<2)c=new Color(.06f,.26f,.37f);texture.SetPixel(gx*cell+x,gy*cell+y,c);}}
 texture.Apply();sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f));var image=GetComponent<Image>();image.sprite=sprite;image.color=Color.white;image.raycastTarget=false;}
 void OnDestroy(){if(sprite)Destroy(sprite);if(texture)Destroy(texture);if(art!=null)art.Dispose();}
}
}
