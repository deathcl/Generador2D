using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Algoritmo
{
    PerlinNoise, PerlinNoiseSuavizado, RandomWalk, RandomWalkSuavizado, PerlinNoiseCueva
}

public class Generador : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap MapaDeLosetas;
    public TileBase Loseta;

    [Header("Dimensiones")]
    public int Ancho = 60;
    public int Alto = 34;

    [Header("Semilla")]
    public float Semilla = 0;
    public bool SemillaAleatoria = true;

    [Header("Algoritmo")]
    public Algoritmo algoritmo = Algoritmo.PerlinNoise;

    [Header("Perlin Noise Suavizado")]
    public int Intervalo = 2;

    [Header("Random Walk Suavizado")]
    public int MinimoAnchoSeccion = 2;

    [Header("Cuevas")]
    public bool LosBordeSonMuros;

    [Header("Perlin Noise Cuevas")]
    public float Modificador = 0.1f;
    public float OffSetX = 0f;
    public float OffSetY = 0f;

    public void GenerarMapa()
    {
        //limpiamos el mapa de losetas
        MapaDeLosetas.ClearAllTiles();
        //creamos el array bidimensional del mapa
        int[,] mapa = null;
        //generamos una semilla nueva aleatoria
        if (SemillaAleatoria)
        {
            Semilla = Random.Range(0f, 1000f);
        }

        switch (algoritmo)
        {
            case Algoritmo.PerlinNoise:
                mapa = Metodos.GenerarArray(Ancho, Alto, true);
                mapa = Metodos.PerlinNoise(mapa, Semilla);
                break;
            case Algoritmo.PerlinNoiseSuavizado:
                mapa = Metodos.GenerarArray(Ancho, Alto, true);
                mapa = Metodos.PerlinNoiseSuavizado(mapa, Semilla, Intervalo);
                break;
            case Algoritmo.RandomWalk:
                mapa = Metodos.GenerarArray(Ancho, Alto, true);
                mapa = Metodos.RandomWalk(mapa, Semilla);
                break;
            case Algoritmo.RandomWalkSuavizado:
                mapa = Metodos.GenerarArray(Ancho, Alto, true);
                mapa = Metodos.RandomWalkSuavizado(mapa, Semilla, MinimoAnchoSeccion);
                break;
            case Algoritmo.PerlinNoiseCueva:
                mapa = Metodos.GenerarArray(Ancho, Alto, false);
                mapa = Metodos.PerlinNoiseCueva(mapa, Modificador, LosBordeSonMuros, OffSetX, OffSetY, Semilla);
                break;
        }

        Metodos.GenerarMapa(mapa, MapaDeLosetas, Loseta);
        //= Metodos.GenerarArray(Ancho, Alto, false); 
        //Metodos.GenerarMapa(mapa, MapaDeLosetas, Loseta);
    }

    public void LimpiarMapa()
    {
        MapaDeLosetas.ClearAllTiles();
    }

}
