using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Algoritmo
{
    PerlinNoise
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
