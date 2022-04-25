using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Effects;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;


/// @author Henri Pigg
/// @version 24.3.2020
///
/// <summary>
/// OHJ1 Harjoitustyö
/// </summary>
public class KerailijaPro : PhysicsGame
{
    private const double liikkumisnopeus = 900;
    private const double paikallaan = 0;
    private Vector paikka = new Vector(0, -390);
    private Vector pelaajanPaikka = new Vector(0, -340);
    private IntMeter pelaajanPisteet;
    private PhysicsObject pelaaja;
    private PhysicsObject kentta;
    private PhysicsObject pallo;
    private int playerHealth = 2;
    private ScoreList topLista = new ScoreList(10, false, 0);
    private IntMeter laskuri = new IntMeter(0);
    private static readonly Image taustaKuva = LoadImage("bossfight1");
    private List<Label> valikonKohdat;
    private const string HIGHSCORE_XML = "highscore.xml";


    /// <summary>
    /// Pääohjelma, jossa kutsutaan alkuvalikko-aliohjelmaa ja ladataan Highscore-listan sisältävä tiedosto.
    /// </summary>
    public override void Begin()
    {
        topLista = DataStorage.Load<ScoreList>(topLista, HIGHSCORE_XML);
        Valikko();
    }


    /// <summary>
    /// Aliohjelma, joka aloittaa pelin ja alustaa kentän.
    /// </summary>
    public void AloitaPeli()
    {
        ClearAll();
        LuoPelaaja(pelaajanPaikka, 140, 80);
        LuoKentta(paikka, 1000, 20);
        PudotaPallot();
        LisaaLaskurit();

        Camera.ZoomToLevel();
        Level.CreateBorders();
        Level.Background.Image = taustaKuva;
    }


    /// <summary>
    /// Aliohjelma, joka luo pelin alkuvalikon.
    /// </summary>
    public void Valikko()
    {
        ClearAll();

        Level.BackgroundColor = Color.Aqua;

        valikonKohdat = new List<Label>();

        Label kohta1 = new Label("Aloita uusi peli");
        kohta1.Position = new Vector(0, 40);
        valikonKohdat.Add(kohta1);

        Label kohta2 = new Label("Parhaat pisteet");
        kohta2.Position = new Vector(0, 0);
        valikonKohdat.Add(kohta2);

        Label kohta3 = new Label("Lopeta peli");
        kohta3.Position = new Vector(0, -40);
        valikonKohdat.Add(kohta3);

        foreach (Label valikonKohta in valikonKohdat)
        {
            Add(valikonKohta);
        }

        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, AloitaPeli, null);
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, ParhaatPisteetAlkuvalikko, null);
        Mouse.ListenOn(kohta3, MouseButton.Left, ButtonState.Pressed, Exit, null);
        Mouse.ListenMovement(1.0, ValikossaLiikkuminen, null);
    }


    /// <summary>
    /// Aliohjelma, kuuntelee hiiren liikkumsita valikossa ja vaihtaa tekstin väriä hiiren paikan mukaan.
    /// </summary>
    /// <param name="hiirenTila">hiiren paikka näytöllä</param>
    public void ValikossaLiikkuminen(AnalogState hiirenTila)
    {
        foreach (Label kohta in valikonKohdat)
        {
            if (Mouse.IsCursorOn(kohta))
            {
                kohta.TextColor = Color.Red;
            }
            else
            {
                kohta.TextColor = Color.Black;
            }
        }
    }


    /// <summary>
    /// Aliohjelma, joka luo Pelaaja-Olion.
    /// </summary>
    /// <param name="PelaajanPaikka">kohta johon pelaaja luodaan kentällä</param>
    /// <param name="leveys">Pelaajan leveys</param>
    /// <param name="korkeus">Pelaajan korkeus</param>
    public void LuoPelaaja(Vector PelaajanPaikka, double leveys, double korkeus)
    {
        pelaaja = new PhysicsObject(leveys, korkeus);
        pelaaja.Shape = Shape.Rectangle;
        pelaaja.Position = PelaajanPaikka;
        pelaaja.KineticFriction = 0.0;
        pelaaja.Mass = 5000;
        pelaaja.CollisionIgnoreGroup = 1;
        pelaaja.Image = LoadImage("betoni");
        Add(pelaaja);

        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikuta pelaajaa oikealle", pelaaja, liikkumisnopeus);
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikuta pelaajaa vasemmalle", pelaaja, -liikkumisnopeus);
        Keyboard.Listen(Key.Right, ButtonState.Released, Liikuta, "pelaaja pysähtyy", pelaaja, paikallaan);
        Keyboard.Listen(Key.Left, ButtonState.Released, Liikuta, "pelaaja pysähtyy", pelaaja, paikallaan);
    }


    /// <summary>
    /// Aliohjelma, joka liikuttaa Pelaaja-Oliota näppäimiä painettaessa.
    /// </summary>
    /// <param name="liikuteltavaOlio">olio, jota liikutetaan</param>
    /// <param name="suunta">vektori, jonka suuntaan liikutaan</param>
    public void Liikuta(PhysicsObject liikuteltavaOlio, double suunta)
    {
        liikuteltavaOlio.Move(new Vector(suunta, 0));
    }


    /// <summary>
    /// Aliohjelma, joka luo pelikentän lattian.
    /// </summary>
    /// <param name="paikka">kenttä-olion paikka pelissä</param>
    /// <param name="leveys">kentän leveys</param>
    /// /// <param name="korkeus">kentän korkeus</param>
    public void LuoKentta(Vector paikka, double leveys, double korkeus)
    {
        kentta = new PhysicsObject(leveys, korkeus);
        kentta.Position = paikka;
        kentta.MakeStatic();
        kentta.Color = Color.Gray;
        kentta.KineticFriction = 0.0;
        kentta.CollisionIgnoreGroup = 1;

        Gravity = new Vector(0.0, -300.0);
        Level.BackgroundColor = Color.Black;
        Add(kentta);
    }


    /// <summary>
    /// Aliohjelma, joka luo putoavia palloja satunnaisella sijainnilla.
    /// </summary>
    public void LuoPallot()
    {
       pallo = new PhysicsObject(100, 100);
        pallo.Shape = Shape.Circle;
        pallo.Y = 350;
        pallo.X = RandomGen.NextDouble(-350, 350);
        pallo.Image = LoadImage("liekki");

        Add(pallo);

        AddCollisionHandler(pallo, PlayersCollided);
    }


    /// <summary>
    /// Ajastin, joka hoitaa putoavien pallojen ajastuksen.
    /// </summary>
    public void PudotaPallot()
    {

        Timer ajastin = new Timer();
        ajastin.Interval = 0.5;
        ajastin.Timeout += LuoPallot;
        ajastin.Start();
    }


    /// <summary>
    /// Aliohjelma, joka luo pistelaskurin.
    /// </summary>
    /// <param name="x">laskurin sijainti x-akselilla</param>
    /// <param name="y">laskurin sijainti y-akselilla</param>
    /// <returns>pistelaskuri</returns>
    public IntMeter LuoPisteLaskuri(double x, double y)
    {
        laskuri = new IntMeter(0);
        laskuri.MaxValue = 1000;

        Label naytto = new Label();
        naytto.BindTo(laskuri);
        naytto.X = x;
        naytto.Y = y;
        naytto.TextColor = Color.White;
        naytto.BorderColor = Level.BackgroundColor;
        naytto.Color = Level.BackgroundColor;
        naytto.Title = "Pisteet";
        Add(naytto);

        return laskuri;
    }


    /// <summary>
    /// Aliohjelma, joka lisää pistelaskurin näytölle.
    /// </summary>
    public void LisaaLaskurit()
    {
        pelaajanPisteet = LuoPisteLaskuri(Screen.Left + 100.0, Screen.Top - 100.0);
    }


    /// <summary>
    /// Aliohjelma, joka tarkastelee fysiikkaolioiden osumia ja suorittaa asioita niiden perusteella.
    /// </summary>
    /// <param name="pallo">olio, joka törmää johonkin</param>
    /// <param name="y">asia, johon törmätään</param>
    public void PlayersCollided(PhysicsObject Pallo, PhysicsObject Kohde)
    {
        if (Kohde == pelaaja)
        { 
            pelaajanPisteet.Value += 1;
            Pallo.Destroy();
        }
        else if (Kohde == kentta)
        {
            Pallo.Destroy();
            playerHealth--;

            if (playerHealth <= 0)
            {
                PelaajaKuoli();
            }
        }
    }


    /// <summary>
    /// Aliohjelma, joka suoritetaan kun pelaajan elämäpsiteet ovat 0.
    /// </summary>
    public void PelaajaKuoli()
    {
        pelaaja.Destroy();
        ParhaatPisteet();
    }


    /// <summary>
    /// Aliohjelma, joka tallentaa pisteet xml-tiedostoon.
    /// </summary>
    public void TallennaPisteet(Window Sender)
    {
        DataStorage.Save<ScoreList>(topLista, HIGHSCORE_XML);
    }


    /// <summary>
    /// Aliohjelma, joka näyttää pisteet pelin jälkeen ja antaa mahdollisuuden lisätä oman nimen listaan.
    /// </summary>
    public void ParhaatPisteet()
    {
        HighScoreWindow topIkkuna = new HighScoreWindow(
                             "Top 10",
                             "Peli ohi! Pääsit listalle pisteillä " + pelaajanPisteet.Value + " Syötä nimesi:",
                             topLista, pelaajanPisteet);
        topIkkuna.Closed += TallennaPisteet; Valikko();
        Add(topIkkuna);
    }


    /// <summary>
    /// Aliohjelma, joka näyttää pisteet alkuvalikossa ilman muokkausmahdollisuuksia.
    /// </summary>
    public void ParhaatPisteetAlkuvalikko()
    {
        HighScoreWindow topIkkuna = new HighScoreWindow(
                             "Top 10",
                             topLista);
        topIkkuna.Closed += null;
        Add(topIkkuna);
    }


}
