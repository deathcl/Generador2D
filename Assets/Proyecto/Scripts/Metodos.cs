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
        for (int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= mapa.GetUpperBound(1); y++)
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
        for (int x = 0; x <= mapa.GetUpperBound(0); x++)
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
                for(int x = posicionAnterior.x; x < posicionActual.x && x <= mapa.GetUpperBound(0); x++)
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

    /// <summary>
    /// Genera el terreno usando el algoritmo Random Walk
    /// </summary>
    /// <param name="mapa">Mapa que modificaremos</param>
    /// <param name="semilla">Semilla que se ultiza para los numeros aleatorios y devuelve el mapa modificado</param>
    /// <returns>Devuelve el mapa modificado con Random Walk</returns>
    public static int[,] RandomWalk(int[,] mapa, float semilla)
    {
        //la semilla de nuestro random
        Random.InitState(semilla.GetHashCode());
        //altura inicial a la que comenzaremos en x
        int ultimaAltura = Random.Range(0, mapa.GetUpperBound(1));
        //recorremos todo el mapa a lo ancho
        for (int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            //0 sube, 1 baja y 2 igual
            int siguienteMovimiento = Random.Range(0, 3);
            //subimos la altura
            if (siguienteMovimiento == 0 && ultimaAltura < mapa.GetUpperBound(1))
            {
                ultimaAltura++;
            }
            //bajamos la altura
            else if (siguienteMovimiento == 1 && ultimaAltura > 0)
            {
                ultimaAltura--;
            }
            //rellenamos el suelo desde la ultima altura hasta abajo
            for (int y = ultimaAltura; y >= 0; y--)
            {
                mapa[x, y] = 1;
            }
        }
        return mapa;
    }

    /// <summary>
    /// Genera el terreno usando el algoritmo Random Walk suavizado
    /// </summary>
    /// <param name="mapa">El mapa que se modificara</param>
    /// <param name="semilla">La semilla de los numeros aleatorios</param>
    /// <param name="miniAnchoSeccion">La minima anchura de la seccion actual antes de cambiar la altura</param>
    /// <returns></returns>
    public static int[,] RandomWalkSuavizado(int[,] mapa, float semilla, int miniAnchoSeccion)
    {
        //la semilla de nuestro random
        Random.InitState(semilla.GetHashCode());
        //altura inicial a la que comenzaremos en x
        int ultimaAltura = Random.Range(0, mapa.GetUpperBound(1));

        //Paara llevar la cuenta del ancho de la seccion actual
        int anchoSeccion = 0;

        //recorremos todo el mapa a lo ancho
        for (int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            if(anchoSeccion >= miniAnchoSeccion)
            {
                //0 sube, 1 baja y 2 igual
                int siguienteMovimiento = Random.Range(0, 3);
                //subimos la altura
                if (siguienteMovimiento == 0 && ultimaAltura < mapa.GetUpperBound(1))
                {
                    ultimaAltura++;
                }
                //bajamos la altura
                else if (siguienteMovimiento == 1 && ultimaAltura > 0)
                {
                    ultimaAltura--;
                }
                //no cambiamos la altura
                anchoSeccion = 0;
            }

            //hemos procesado otro bloque de la seccion actual
            anchoSeccion++;

            //rellenamos el suelo desde la ultima altura hasta abajo
            for (int y = ultimaAltura; y >= 0; y--)
            {
                mapa[x, y] = 1;
            }
        }
        return mapa;
    }

    /// <summary>
    /// Genera una cueva utilizando el algoritmo Perlin Noise
    /// </summary>
    /// <param name="mapa">El mapa que se va a modificar</param>
    /// <param name="modificador">Valor por el cual multiplicamos la posicion para obtener un valor del Perlin Noise</param>
    /// <param name="losBordesSonMuro">Si vale verdadero los bordes son muros, sino no existen muros</param>
    /// <param name="offSetX">Desplaamiento en X para el Perlin Noise</param>
    /// <param name="offSetY">Desplazamiento en Y para el Perlin Noise</param>
    /// <param name="semilla">La semilla que se usa para situarse en x,y en el Perlin Noise</param>
    /// <returns>El mapa con la cueva generada con el Perlin Noise</returns>
    public static int[,] PerlinNoiseCueva(int[,] mapa, float modificador, bool losBordesSonMuro, float offSetX = 0f, float offSetY = 0f, float semilla = 0f)
    {
        int nuevoPunto;
        for (int x = 0; x <= mapa.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= mapa.GetUpperBound(1); y++)
            {
                if(losBordesSonMuro && (x == 0 || y == 0 || x == mapa.GetUpperBound(0) || y == mapa.GetUpperBound(1)))
                {
                    mapa[x, y] = 1;
                }
                else
                {
                    nuevoPunto= Mathf.RoundToInt(Mathf.PerlinNoise(x * modificador + offSetX + semilla, y * modificador + offSetY + semilla));
                    mapa[x, y] = nuevoPunto;
                }
            }
        }
        return mapa;
    }
}
