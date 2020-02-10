import java.io.*;

public class Main 
{

	@SuppressWarnings("resource")
	public static void main(String[] args) throws IOException 
	{
		// Lexer (TODO)
		System.out.flush();
		if (args.length > 0)
		{
			String filename = args[0];
			File file = new File(filename);   
			BufferedReader br = new BufferedReader(new FileReader(file)); 
			String current_line; 
			while ((current_line = br.readLine()) != null) 
			{
				System.out.println(current_line); 
			} 
		}
	}

}
