using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public static class Helper
{
    /// <summary>
    /// Bỏ các số sau dấu phẩy của số thập
    /// </summary>
    /// <param name="Round">Số chữ số sau dấu phẩy</param>
    /// <returns></returns>
    public static float DecimalRounding(float Num, int Round)
    {
        Num = ((int)(Math.Pow(10, Round) * Num)) / (float)Math.Pow(10, Round);
        return Num;
    }

    public static T[] DisruptiveArrayObject<T>(T[] array) where T : struct
    {
        for (int i = 0; i < array.Length; i++)
        {
            //int r1 = System.Random.Next
            int r1 = UnityEngine.Random.Range(0, array.Length);
            int r2 = UnityEngine.Random.Range(0, array.Length);

            T temp = array[r1];
            array[r1] = array[r2];
            array[r2] = temp;
        }

        return array;
    }

    
    public static List<object> DisruptiveListObject(List<object> array)
    {
        for (int i = 0; i < array.Count; i++)
        {
            //int r1 = System.Random.Next
            int r1 = UnityEngine.Random.Range(0, array.Count);
            int r2 = UnityEngine.Random.Range(0, array.Count);

            object temp = array[r1];
            array[r1] = array[r2];
            array[r2] = temp;
        }
        
        return array;
    }

    public static List<T> DisruptiveList<T>(List<T> array) where T : MonoBehaviour
    {
        for (int i = 0; i < array.Count; i++)
        {
            //int r1 = System.Random.Next
            int r1 = UnityEngine.Random.Range(0, array.Count);
            int r2 = UnityEngine.Random.Range(0, array.Count);

            T temp = array[r1];
            array[r1] = array[r2];
            array[r2] = temp;
        }

        return array;
    }

    public static List<T> DisruptiveListObject<T>(List<T> array) where T : struct
    {
        for (int i = 0; i < array.Count; i++)
        {
            //int r1 = System.Random.Next
            int r1 = UnityEngine.Random.Range(0, array.Count);
            int r2 = UnityEngine.Random.Range(0, array.Count);

            T temp = array[r1];
            array[r1] = array[r2];
            array[r2] = temp;
        }

        return array;
    }

    public static List<T> DisruptiveListSelf<T>(List<T> array) where T : SelfDefine
    {
        for (int i = 0; i < array.Count; i++)
        {
            //int r1 = System.Random.Next
            int r1 = UnityEngine.Random.Range(0, array.Count);
            int r2 = UnityEngine.Random.Range(0, array.Count);

            T temp = array[r1];
            array[r1] = array[r2];
            array[r2] = temp;
        }

        return array;
    }

    public delegate int Compare(object a1, object a2);

    
    /// <summary>
    ///Sắp xếp giảm dần
    /// </summary>
    /// <param name="A"></param>
    /// <returns></returns>
    public static List<object> SelectionSortList( List<object> A, Compare compareElementList)
    {
        int i, j, min_idx;
        int n = A.Count;

        for (i = 0; i < n - 1; i++)
        {
            // Tìm phần tử nhỏ nhất trong mảng
            min_idx = i;
            for (j = i + 1; j < n; j++)
                if (compareElementList(A[j], A[min_idx]) < 0)
                    min_idx = j;

            // Đổi chỗ phần tử nhỏ nhất trong mảng
            object temp = A[min_idx];
            A[min_idx] = A[i];
            A[i] = temp;
        }

        return A;
    }

    /// <summary>
    /// Gọi một Hàm chạy liên tục
    /// </summary>
    /// <param name="action"></param>
    /// <param name="conditionStop"></param>
    /// <returns></returns>
    public static IEnumerator StartThread(UnityAction action, System.Func<bool> conditionStop,
        UnityAction actionStop = null, float time = 0.01f)
    {
        bool isStop = false;

        while (isStop == false)
        {
            yield return new WaitForSecondsRealtime(time);
            // Debug.Log("conditionStop " + conditionStop());
            if (conditionStop() == true)
            {
                isStop = true;
                if (actionStop != null)
                    actionStop();
            }
            else
            {
                action();
            }
        }
    }

    /// <summary>
    /// Gọi một Hàm chạy liên tục
    /// </summary>
    /// <param name="action"></param>
    /// <param name="conditionStop"></param>
    /// <returns></returns>
    public static IEnumerator StartThread(UnityAction action, float timeToStop, UnityAction actionStop = null)
    {
        bool isStop = false;

        float timer = 0;

        while (isStop == false)
        {
            yield return new WaitForSeconds(0.02f);
            timer += 0.02f;
            //Debug.Log(timer);
            if (timer >= timeToStop - 1)
            {
                isStop = true;
                if (actionStop != null)
                    actionStop();
            }
            else
                action();
        }
    }

    /// <summary>
    /// Tạo một hàm chạy sau một khoảng thời gian
    /// </summary>
    /// <param name="action"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IEnumerator StartAction(UnityAction action, float time)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public static void StartActionNotUseCorutines(UnityAction action, float time)
    {
        Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(l => { action(); });
    }

    /// <summary>
    /// Tạo một hàm chạy khi một điều kiện thỏa mãn
    /// </summary>
    /// <param name="action"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IEnumerator StartAction(UnityAction action, System.Func<bool> condition)
    {
        yield return new WaitUntil(condition);
        action();
    }

    /// <summary>
    /// Lấy một điểm cách 1 điểm cho trước và theo 1 hướng
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="direction"></param>
    /// <param name="fromPoint"></param>
    /// <returns></returns>
    public static Vector3 GetPointDistanceFromObject(float distance, Vector3 direction, Vector3 fromPoint)
    {
        distance -= 1;
        //if (distance < 0)
        //    distance = 0;

        Vector3 finalDirection = direction + direction.normalized * distance;
        Vector3 targetPosition = fromPoint + finalDirection;

        return targetPosition;
    }


    /// <summary>
    /// Lấy ra một Vector hợp với VectorP một góc angle. Điểm đầu của Vector là PositionStart
    /// </summary>
    /// <param name="vectorP"></param>
    /// <param name="angle"></param>
    /// <param name="positionStart"></param>
    /// <returns></returns>
    public static Vector3 GetDirectionFromAngle(Vector3 vectorP, float angle, Vector3 positionStart)
    {
        if (angle == 90)
        {
            return new Vector3(vectorP.y, -vectorP.x).normalized;
        }
        else if (angle == 0)
        {
            return vectorP;
        }
        else if (angle == 180)
        {
            return -vectorP;
        }
        else if (angle == 360)
        {
            return new Vector3(-vectorP.y, vectorP.x).normalized;
        }

        if (angle > 360)
            angle -= 360;
        if (angle < 0)
            angle *= -1;

        float radiaAngle = (angle * Mathf.PI) / 180;
        float tanAngle = Mathf.Tan(radiaAngle);
        Vector2 u = new Vector2(vectorP.y, -vectorP.x);

        Vector3 B = GetPointDistanceFromObject(1, vectorP, positionStart);
        //Debug.Log("Distance "+ Vector3.Distance(positionStart, B));

        Vector3 result = GetPointDistanceFromObject(tanAngle, u, B);

        return (result - positionStart).normalized;
    }

    /// <summary>
    /// Lấy ra một Vector hợp với VectorP một góc angle. Điểm đầu của Vector là PositionStart
    /// </summary>
    /// <param name="vectorP"></param>
    /// <param name="angle"></param>
    /// <param name="positionStart"></param>
    /// <returns></returns>
    public static Vector3 GetDirectionFromAngle_2(Vector3 vectorP, float angle, Vector3 positionStart)
    {
        if (angle == 90)
        {
            return new Vector3(vectorP.y, -vectorP.x).normalized;
        }
        else if (angle == 0)
        {
            return vectorP;
        }
        else if (angle == 180)
        {
            return -vectorP;
        }
        else if (angle == 360)
        {
            return new Vector3(-vectorP.y, vectorP.x).normalized;
        }

        if (angle > 360)
            angle -= 360;
        if (angle < 0)
            angle *= -1;

        if (angle > 90 && angle < 270)
            vectorP = -vectorP;

        float radiaAngle = (angle * Mathf.PI) / 180;
        float tanAngle = Mathf.Tan(radiaAngle);
        Vector2 u = new Vector2(vectorP.y, -vectorP.x);

        Vector3 B = GetPointDistanceFromObject(1, vectorP, positionStart);
        //Debug.Log("Distance "+ Vector3.Distance(positionStart, B));

        Vector3 result = GetPointDistanceFromObject(tanAngle, u, B);

        return (result - positionStart).normalized;
    }

    public static IEnumerator DOLocalRotateQuaternion(Quaternion endValue, float speedRotate, GameObject objLookAt,
        UnityAction actionComplete)
    {
        bool isStop = false;

        while (isStop == false)
        {
            yield return new WaitForSecondsRealtime(Time.deltaTime);
            //Debug.Log(objLookAt.name + " "   + Quaternion.Angle(objLookAt.transform.localRotation, endValue));
            if (Quaternion.Angle(objLookAt.transform.localRotation, endValue) <= 8f)
            {
                isStop = true;
                break;
            }
            else
            {
                objLookAt.transform.localRotation = Quaternion.Slerp(objLookAt.transform.localRotation, endValue,
                    Time.deltaTime * speedRotate);
            }
        }

        actionComplete();
    }

    // New Function added here
    // assume you only care about the y Axis rotation
    // can change this to care about other Axis
    // return true if rotating clockwise
    // return false if rotating counterclockwise
    public static bool GetRotateDirection(Quaternion from, Quaternion to)
    {
        float fromY = from.eulerAngles.y;
        float toY = to.eulerAngles.y;
        float clockWise = 0f;
        float counterClockWise = 0f;

        if (fromY <= toY)
        {
            clockWise = toY - fromY;
            counterClockWise = fromY + (360 - toY);
        }
        else
        {
            clockWise = (360 - fromY) + toY;
            counterClockWise = fromY - toY;
        }
        return (clockWise <= counterClockWise);
    }

    public static void LookAtToDirection(Vector3 diretion, GameObject objLookAt, float speedLockAt = 500)
    {
        float xTarget = diretion.x;
        float yTarget = diretion.y;
        float angle = Mathf.Atan2(yTarget, xTarget) * Mathf.Rad2Deg + 90;
        
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

      

        objLookAt.transform.rotation = Quaternion.Slerp(objLookAt.transform.rotation, q, Time.deltaTime * speedLockAt);
    }

    public static void LookAtToDirectionAngleDetal(Vector3 diretion, GameObject objLookAt, float AngleDetal, float speedLockAt = 500)
    {
        float xTarget = diretion.x;
        float yTarget = diretion.y;
        float angle = Mathf.Atan2(yTarget, xTarget) * Mathf.Rad2Deg + 90 + AngleDetal;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        objLookAt.transform.rotation = Quaternion.Slerp(objLookAt.transform.rotation, q, Time.deltaTime * speedLockAt);
    }

    public static void LookAtToPosition(Vector3 position, GameObject objLookAt, float speedRotate = 500)
    {
        float xTarget = position.x - objLookAt.transform.position.x;
        float yTarget = position.y - objLookAt.transform.position.y;
        float angle = Mathf.Atan2(yTarget, xTarget) * Mathf.Rad2Deg + 90;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        objLookAt.transform.rotation = Quaternion.Slerp(objLookAt.transform.rotation, q, Time.deltaTime * speedRotate);
    }

    public static void LookAtToPositionAngleDetal(Vector3 position, GameObject objLookAt, float AngleDetal, float speedRotate = 500)
    {
        float xTarget = position.x - objLookAt.transform.position.x;
        float yTarget = position.y - objLookAt.transform.position.y;
        float angle = Mathf.Atan2(yTarget, xTarget) * Mathf.Rad2Deg + 90 + AngleDetal;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        objLookAt.transform.rotation = Quaternion.Slerp(objLookAt.transform.rotation, q, Time.deltaTime * speedRotate);
    }
    
    public static int RandomCustom(int first, int last, List<int> diff)
    {
        int r = -1;
        while (r < 0)
        {
            r = UnityEngine.Random.Range(first, last);
            for (int i = 0; i < diff.Count; i++)
            {
                if (r == diff[i])
                {
                    r = -1;
                    break;
                }
            }
        }
        return r;
    }

    /// <summary>
    /// Random một phần tử trong 1 mảng không trùng lặp với 1 phần tử cho trước
    /// </summary>
    /// <param name="oldValue">Giá trị mẫu không được trùng lặp</param>
    /// <param name="diff">List giá trị</param>
    /// <returns></returns>
    public static int RandomUnduplicated(int oldValue, List<int> diff)
    {
        List<int> m_diff = new List<int>(diff);

        //Trộn List
        m_diff = DisruptiveListObject(m_diff);

        for (int i = 0; i < m_diff.Count; i++)
        {
            if (oldValue != m_diff[i])
                return m_diff[i];
        }

        return oldValue;
    }

    /// <summary>
    /// Random một phần tử trong 1 mảng không trùng lặp với 1 phần tử cho trước
    /// </summary>
    /// <param name="oldValues">Giá trị mẫu không được trùng lặp</param>
    /// <param name="diff">List giá trị</param>
    /// <returns></returns>
    public static int RandomUnduplicated(List<int> diff, params int[] oldValues)
    {
        List<int> m_diff = new List<int>(diff);

        //Trộn List
        m_diff = DisruptiveListObject(m_diff);

        for (int i = 0; i < m_diff.Count; i++)
        {
            bool isHasSame = false;
            for (int j = 0; j < oldValues.Length; j++)
            {
                if (oldValues[j] == m_diff[i])
                {
                    isHasSame = true;
                    break;
                }
            }

            if(!isHasSame)
                return m_diff[i];
        }

        return 0;
    }

    //public static List<HPInPattern> SelectionSortPattern(List<HPInPattern> A)
    //{
    //    int i, j, min_idx;
    //    int n = A.Count;

    //    for (i = 0; i < n - 1; i++)
    //    {
    //        // Tìm phần tử nhỏ nhất trong mảng
    //        min_idx = i;
    //        for (j = i + 1; j < n; j++)
    //            if ((int)A[j].typePattern < (int)A[min_idx].typePattern)
    //                min_idx = j;

    //        // Đổi chỗ phần tử nhỏ nhất trong mảng
    //        HPInPattern temp = A[min_idx];
    //        A[min_idx] = A[i];
    //        A[i] = temp;
    //    }

    //    return A;
    //}

    public static IObservable<WWW> DownloadOrCache(string url, string fileID)
    {
        string downloadUrl = getCachedWWW(url, fileID, out var fileUrl);
        return ObservableWWW.GetWWW(downloadUrl).Do(_ =>
        {
            if (string.IsNullOrEmpty(_.error))
            {
                File.WriteAllBytes(fileUrl, _.bytes);
            }
        }).CatchIgnore();
    }
    
    public static string getCachedWWW(string url, string fileID, out string fileUrl)
    {
        if (string.IsNullOrEmpty(fileID))
        {
            fileID = "temp";
        }
        string filePath = Application.persistentDataPath;
        filePath += "/" + Convert.ToBase64String(Encoding.UTF8.GetBytes(fileID));
        string pathforwww;
        if (File.Exists(filePath))
        {
            pathforwww = "file://" + filePath;
            //Debug.Log("TRYING FROM CACHE " + url + "  file " + pathforwww);
        }
        else
        {
            pathforwww = url;
        }
        fileUrl = filePath;
        return pathforwww;
    }
}

public class SelfDefine : MonoBehaviour
{

}

public static class MMMaths
{
    /// <summary>
    /// Takes a Vector3 and turns it into a Vector2
    /// </summary>
    /// <returns>The vector2.</returns>
    /// <param name="target">The Vector3 to turn into a Vector2.</param>
    public static Vector2 Vector3ToVector2(Vector3 target)
    {
        return new Vector2(target.x, target.y);
    }

    /// <summary>
    /// Takes a Vector2 and turns it into a Vector3 with a null z value
    /// </summary>
    /// <returns>The vector3.</returns>
    /// <param name="target">The Vector2 to turn into a Vector3.</param>
    public static Vector3 Vector2ToVector3(Vector2 target)
    {
        return new Vector3(target.x, target.y, 0);
    }

    /// <summary>
    /// Takes a Vector2 and turns it into a Vector3 with the specified z value 
    /// </summary>
    /// <returns>The vector3.</returns>
    /// <param name="target">The Vector2 to turn into a Vector3.</param>
    /// <param name="newZValue">New Z value.</param>
    public static Vector3 Vector2ToVector3(Vector2 target, float newZValue)
    {
        return new Vector3(target.x, target.y, newZValue);
    }

    /// <summary>
    /// Returns a random vector3 from 2 defined vector3.
    /// </summary>
    /// <returns>The vector3.</returns>
    /// <param name="min">Minimum.</param>
    /// <param name="max">Maximum.</param>
    public static Vector3 RandomVector3(Vector3 minimum, Vector3 maximum)
    {
        return new Vector3(UnityEngine.Random.Range(minimum.x, maximum.x),
                                         UnityEngine.Random.Range(minimum.y, maximum.y),
                                         UnityEngine.Random.Range(minimum.z, maximum.z));
    }

    /// <summary>
    /// Rotates a point around the given pivot.
    /// </summary>
    /// <returns>The new point position.</returns>
    /// <param name="point">The point to rotate.</param>
    /// <param name="pivot">The pivot's position.</param>
    /// <param name="angle">The angle we want to rotate our point.</param>
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
    {
        angle = angle * (Mathf.PI / 180f);
        var rotatedX = Mathf.Cos(angle) * (point.x - pivot.x) - Mathf.Sin(angle) * (point.y - pivot.y) + pivot.x;
        var rotatedY = Mathf.Sin(angle) * (point.x - pivot.x) + Mathf.Cos(angle) * (point.y - pivot.y) + pivot.y;
        return new Vector3(rotatedX, rotatedY, 0);
    }

    /// <summary>
    /// Rotates a point around the given pivot.
    /// </summary>
    /// <returns>The new point position.</returns>
    /// <param name="point">The point to rotate.</param>
    /// <param name="pivot">The pivot's position.</param>
    /// <param name="angles">The angle as a Vector3.</param>
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
    {
        // we get point direction from the point to the pivot
        Vector3 direction = point - pivot;
        // we rotate the direction
        direction = Quaternion.Euler(angle) * direction;
        // we determine the rotated point's position
        point = direction + pivot;
        return point;
    }

    /// <summary>
    /// Returns the sum of all the int passed in parameters
    /// </summary>
    /// <param name="thingsToAdd">Things to add.</param>
    public static int Sum(params int[] thingsToAdd)
    {
        int result = 0;
        for (int i = 0; i < thingsToAdd.Length; i++)
        {
            result += thingsToAdd[i];
        }
        return result;
    }

    /// <summary>
    /// Returns the result of rolling a dice of the specified number of sides
    /// </summary>
    /// <returns>The result of the dice roll.</returns>
    /// <param name="numberOfSides">Number of sides of the dice.</param>
    public static int RollADice(int numberOfSides)
    {
        return (UnityEngine.Random.Range(1, numberOfSides));
    }

    /// <summary>
    /// Returns a random success based on X% of chance.
    /// Example : I have 20% of chance to do X, Chance(20) > true, yay!
    /// </summary>
    /// <param name="percent">Percent of chance.</param>
    public static bool Chance(int percent)
    {
        return (UnityEngine.Random.Range(0, 100) <= percent);
    }

    /// <summary>
    /// Moves from "from" to "to" by the specified amount and returns the corresponding value
    /// </summary>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    /// <param name="amount">Amount.</param>
    public static float Approach(float from, float to, float amount)
    {
        if (from < to)
        {
            from += amount;
            if (from > to)
            {
                return to;
            }
        }
        else
        {
            from -= amount;
            if (from < to)
            {
                return to;
            }
        }
        return from;
    }
}