using UnityEngine;
using UnityEngine.UI;
namespace SokobanStudio {
public sealed class LevelPreview:MonoBehaviour {
 Texture2D texture;WarehouseArt art;
 public void Bind(LevelData d){if(texture)Destroy(texture);if(art!=null)art.Dispose();art=new WarehouseArt();const int cell=16;texture=new Texture2D(d.width*cell,d.height*cell);texture.filterMode=FilterMode.Point;for(int y=0;y<d.height;y++)for(int x=0;x<d.width;x++){var c=new Cell(x,y);string terrain=d.terrain[y*d.width+x]==Terrain.Wall?WarehouseArt.WallKey(d,c):"floor";for(int py=0;py<cell;py++)for(int px=0;px<cell;px++){Color result=StudioUI.Background;if(d.terrain[y*d.width+x]!=Terrain.Void){Blend(ref result,terrain,px,py);if(System.Array.IndexOf(d.goals,c)>=0)Blend(ref result,"goal",px,py);if(System.Array.IndexOf(d.boxes,c)>=0)Blend(ref result,System.Array.IndexOf(d.goals,c)>=0?"docked":"crate",px,py);if(d.player.Equals(c))Blend(ref result,"robot",px,py);}texture.SetPixel(x*cell+px,(d.height-1-y)*cell+py,result);}}texture.Apply();var image=GetComponent<RawImage>()??gameObject.AddComponent<RawImage>();image.texture=texture;image.raycastTarget=false;var fit=GetComponent<AspectRatioFitter>()??gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=d.width/(float)d.height;}
 void Blend(ref Color color,string kind,int x,int y){var sample=art.Art(kind).texture.GetPixel(x*4+2,y*4+2);color=Color.Lerp(color,sample,sample.a);}
 void OnDestroy(){if(texture)Destroy(texture);if(art!=null)art.Dispose();}
 public static void Attach(RectTransform host,LevelData d){var r=StudioUI.Rect(host,"Map Preview",Vector2.zero,Vector2.one,Vector2.zero);r.gameObject.AddComponent<LevelPreview>().Bind(d);}
}
public class MenuWarehouse:MonoBehaviour {
 WarehouseArt art;Image eye,goal;float time;
 void Start(){art=new WarehouseArt();for(int y=0;y<5;y++)for(int x=0;x<6;x++){float left=.06f+x*.145f,bottom=.14f+y*.14f;Part("floor",left,bottom,left+.14f,bottom+.135f);if(y==4||x==5&&y>1)Part("wall",left,bottom,left+.14f,bottom+.135f);}goal=Part("goal",.5f,.42f,.64f,.56f);Part("crate",.35f,.42f,.49f,.56f);Part("crate",.65f,.14f,.79f,.28f);Part("docked",.65f,.28f,.79f,.42f);eye=Part("robot",.2f,.42f,.34f,.56f);var label=StudioUI.Rect(transform,"Dock label",new Vector2(.06f,.02f),new Vector2(.94f,.12f),Vector2.zero);StudioUI.Text(label,"DOCK 07   /   CARGO OPERATIONS",16);}
 Image Part(string kind,float x,float y,float x2,float y2){var r=StudioUI.Rect(transform,kind,new Vector2(x,y),new Vector2(x2,y2),Vector2.zero);var i=r.gameObject.AddComponent<Image>();i.sprite=art.Art(kind);i.preserveAspect=true;i.raycastTarget=false;return i;}
 void Update(){time+=Time.unscaledDeltaTime;if(goal)goal.color=new Color(1,1,1,.65f+.35f*Mathf.Sin(time*2));if(eye)eye.sprite=art.Art(time%3.8f<.16f?"robotblink":"robot");}
 void OnDestroy(){if(art!=null)art.Dispose();}
}
}
