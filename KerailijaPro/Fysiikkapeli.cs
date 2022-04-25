using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class KerailijaPro : PhysicsGame
{
    private const double liikkumisnopeus = 500;
    private const double paikallaan = 0;
    Vector paikka = new Vector(0, -390);
    Vector PelaajanPaikka = new Vector(0, -340);


    Image taustaKuva = LoadImage("bossfight1");
    public override void Begin()
    {
        Level.Background.Image = taustaKuva;
        LuoPelaaja(PelaajanPaikka, 160, 80);
        LuoKentta(paikka, 1000, 20);
        LuoPallot();
        LisaaLaskurit();

        Camera.ZoomToLevel();
        Level.CreateBorders();
   
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    public void LuoPelaaja(Vector PelaajanPaikka, double leveys, double korkeus)
    {
        PhysicsObject Pelaaja = new PhysicsObject(leveys, korkeus);
        Pelaaja.Shape = Shape.Rectangle;
        Pelaaja.Position = PelaajanPaikka;
        Pelaaja.KineticFriction = 0.0;
        Pelaaja.Mass = 5000;
        Pelaaja.CollisionIgnoreGroup = 1;
        Add(Pelaaja);

        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikuta pelaajaa oikealle", Pelaaja, liikkumisnopeus);
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikuta pelaajaa vasemmalle", Pelaaja, -liikkumisnopeus);
        Keyboard.Listen(Key.Right, ButtonState.Released, Liikuta, "pelaaja pysähtyy", Pelaaja, paikallaan);
        Keyboard.Listen(Key.Left, ButtonState.Released, Liikuta, "pelaaja pysähtyy", Pelaaja, paikallaan);
    }


    public void Liikuta(PhysicsObject liikuteltavaOlio, double suunta)
    {
        liikuteltavaOlio.Move(new Vector(suunta, 0));
    }


    public void LuoKentta(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject kentta = new PhysicsObject(leveys, korkeus);
        kentta.Position = paikka;
        kentta.MakeStatic();
        kentta.Color = Color.Gray;
        kentta.KineticFriction = 0.0;
        kentta.CollisionIgnoreGroup = 1;

        Gravity = new Vector(0.0, -100.0);
        Level.BackgroundColor = Color.Black;
        Add(kentta);

    }


    public void LuoPallot()
    {
        PhysicsObject Pallo = new PhysicsObject(100, 100);
        Pallo.Shape = Shape.Circle;
        Pallo.Y = 350;
        Pallo.X = RandomGen.NextDouble(-300, 300);
        Pallo.Image = LoadImage("liekki");
        
        Add(Pallo);

        AddCollisionHandler(Pallo, PlayersCollided);
    }


    public void PudotaPallot()
    {
        
    }


    IntMeter LuoPisteLaskuri(double x, double y)
    {
        IntMeter laskuri = new IntMeter(0);
        laskuri.MaxValue = 1000;

        Label naytto = new Label();
        naytto.BindTo(laskuri);
        naytto.X = x;
        naytto.Y = y;
        naytto.TextColor = Color.White;
        naytto.BorderColor = Level.BackgroundColor;
        naytto.Color = Level.BackgroundColor;
        Add(naytto);

        return laskuri;
    }

    public void LisaaLaskurit()
    {
        IntMeter PelaajanPisteet = LuoPisteLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);

    }

    public void PlayersCollided(PhysicsObject Pallo, PhysicsObject Pelaaja)
    {
        Pallo.Destroy();

        if (Pallo == Pelaaja)
        {
           
        }
   
    }


}
