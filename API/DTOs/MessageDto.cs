namespace API;

public class MessageDto
{


public int Id { get; set; }
public int SenderId { get; set; }
public string SenderUsername { get; set; }

public string SenderPhotoUrl { get; set; }


public int RecipientId { get; set; }
public string RecipientUsername { get; set; }

public string RecepientPhotoUrl { get; set; }
 public string Content { get; set; }

 public DateTime? DateRead  { get; set; }

 public DateTime MessageSent { get; set; } =DateTime.UtcNow;

 public Boolean SenderDeleted { get; set; }
 public Boolean RecipientDeleted { get; set; }


}
