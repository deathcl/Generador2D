using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Metodos
{
    /// <summary>
    /// Genera un array bidimiensional
    /// </summary>
    /// <param name="ancho">Ancho del mapa 2D</param>
    /// <param name="alto">Alto del mapa 2D</param>
    /// <param name="vacio">True inicializara todo a cero. False inicializara todo a 1.</param>
    /// <returns>El mapa 2D generado.</returns>
    public static int[,] GenerarArray(int ancho, int alto, bool vacio)
    {
        int[,] mapa = new int[ancho, alto];
        for(int x = 0; x < ancho; x++)
        {
            for(int y = 0; y < alto; y++)
            {
                mapa[x,y] = vacio ? 0 : 1;
            }
        }
        return mapa;
    }

    /// <summary>
    /// Genera el mapa de losetas con la info ubicada en "mapa"
    /// </summary>
    /// <param name="mapa">info que se utilizara el mapa de losetas. 1 hay losetas, o no hay losetas.</param>
    /// <param name="mapaDeLosetas">Referencia al mapa de losetas donde se generaran las losetas.</param>
    /// <param name="loseta">Loseta que se utilizara para pintar el suelo en el mapa de losetas.</param>
    public static void GenerarMapa(int[,] mapa, Tilemap mapaDeLosetas, TileBase loseta)
    {
        //limpiamos el mapa de casillas para comenzar con uno vacio
        mapaDeLosetas.ClearAllTiles();
        for (int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            for (int y = 0; y < mapa.GetUpperBound(1); y++)
            {
                //1 = hay suelo, 0 = no hay suelo
                if (mapa[x,y] == 1)
                {
                    mapaDeLosetas.SetTile(new Vector3Int(x, y, 0), loseta);
                }
            }
        }
    }

    /// <summary>
    /// Genera el terreno basandose en el algoritmo Perlin Noise    
    /// </summary>
    /// <param name="mapa">Array a modificar donde se guardara el terreno generado</param>
    /// <param name="semilla">Semilla que se utilizara para generar el terreno</param>
    /// <returns>Array modificado con el terreno generado</returns>
    public static int[,] PerlinNoise(int[,] mapa, float semilla)
    {
        //altura del punto actual en x
        int nuevoPunto;

        //como mathf.perlinnoise devuelve un valor entre 0 y 1 le restamos 
        //esta variable para que el valor final este entre -0.5 y 0.5
        float reduccion = 0.5f;
        //crear el perlin noise
        for (int x = 0; x < mapa.GetUpperBound(0); x++)
        {
            nuevoPunto = Mathf.FloorToInt((Mathf.PerlinNoise(x, semilla) - reduccion) * mapa.GetUpperBound(1));
            nuevoPunto += (mapa.GetUpperBound(1) / 2);

            for (int y = nuevoPunto; y >= 0; y--)
            {
                mapa[x, y] = 1;
            }
        }

        return mapa;
    }

    /// <summary>
    /// Modifica el mapa creando un terreno Perlin Noise suavizado
    /// </summary>
    /// <param name="mapa">El mapa que vamos a editar</param>
    /// <param name="semilla">La semilla para la generacion del Perlin Noise</param>
    /// <param name="intervalo">El intervalo en el que grabaremos la altura</param>
    /// /// <returns>Devuelve el mapa modificado</returns>
    public static int[,] PerlinNoiseSuavizado(int[,] mapa, float semilla, int intervalo)
    {
        if(intervalo > 1)
        {
            //utilizados en el proceso de suavizado
            Vector2Int posicionActual, posicionAnterior;
            //Los puntos correspondientes para el suevizado, uno en cada eje.
            List<int> ruidoX = new List<int>();
            List<int> ruidoY = new List<int>();

            int nuevoPunto, puntos;

            //Genera el ruido (guarda la altura de ciertos picos (segun el intervalo) del mapa generado)
            for (int x = 0; x <= mapa.GetUpperBound(0) + intervalo; x += intervalo)
            {
                nuevoPunto = Mathf.FloorToInt(Mathf.PerlinNoise(x, semilla) * mapa.GetUpperBound(1));
                ruidoY.Add(nuevoPunto);
                ruidoX.Add(x);
            }
            puntos = ruidoY.Count;

            //Empezamos en la primera posicion para asi tener disponible una posicion anterior
            for(int i = 1; i < puntos; i++)
            {
                //OBtenemos la posicion antual
                posicionActual = new Vector2Int(ruidoX[i], ruidoY[i]);
                //Obtenemos la posicion anterior
                posicionAnterior = new Vector2Int(ruidoX[i - 1], ruidoY[i - 1]);

                //Diferencia entre las dos posiciones. Cuanto se ha movido entre altura
                Vector2 diferencia = posicionActual - posicionAnterior;

                //Calculamos el cambio de altura
                float cambioEnAltura = diferencia.y / intervalo;
                //guardamos la altura actual
                float alturaActual = posicionAnterior.y;

                //avanza de la posicion anterior hazta la posicion actual
                for(int x = posicionAnterior.x; x < posicionActual.x && x < mapa.GetUpperBound(0); x++)
                {
                    //vamos dibujando losetas desde la altura actual
                    for(int y = Mathf.FloorToInt(alturaActual); y >=
                        0; y--)
                    {
                        mapa[x, y] = 1;
                    }
                    //calculamos altura actual
                    alturaActual += cambioEnAltura;
                }
            }
        }
        else
        {
            mapa =  PerlinNoise(mapa, semilla);
        }

        return mapa;
    }
}
