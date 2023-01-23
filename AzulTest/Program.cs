// If I pass in [2, 1, 5, 1, 4, 7, 4], it should return [2, 5, 7]
// Write a method that takes in this array and returns a new array without duplicates, and in the same order.

static void Main(string[] args)
{    
    int[] array = new int[] { 4, 8, 4, 1, 1, 4, 8 };            
    int numDups = 0, prevIndex = 0;

    for (int i = 0; i < array.Length; i++)
    {
        bool foundDup = false;
        for (int j = 0; j < i; j++)
        {
            if (array[i] == array[j])
            {
                foundDup = true;
                numDups++; // Increment means Count for Duplicate found in array.
                break;
            }                    
        }

        if (foundDup == false)
        {
            array[prevIndex] = array[i];
            prevIndex++;
        }
    }

    // Just Duplicate records replce by zero.
    for (int k = 1; k <= numDups; k++)
    {               
        array[array.Length - k] = '\0';             
    }
    
}