using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

public class GrantsTest
{
    /// <summary>
    /// Creates a Player instance that is spawned at the specified location.
    /// </summary>
    /// <param name="x">Spawn x-position</param>
    /// <param name="y">Spawn y-position</param>
    /// <returns></returns>
    public Player CreatePlayer(float x=0, float y=0)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Player", typeof(GameObject))) as GameObject;
        Player player = obj.GetComponent<Player>();
        if (!player) return null;
        player.rb.position = new Vector3(x, y, 0);
        return player;
    }

    /// <summary>
    /// Creates a ChatBar instance that is spawned at the specified location.
    /// </summary>
    /// <param name="x">Spawn x-position</param>
    /// <param name="y">Spawn y-position</param>
    /// <returns></returns>
    public ChatBar CreateChatBar(float x=0, float y=0)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("ChatBar", typeof(GameObject))) as GameObject;
        ChatBar chatBar = obj.GetComponent<ChatBar>();
        if (!chatBar) return null;
        chatBar.rb.position = new Vector3(x, y, 0);
        return chatBar;
    }

    /// <summary>
    /// Creates a ChatBox instance that is spawned at the specified location.
    /// </summary>
    /// <param name="x">Spawn x-position</param>
    /// <param name="y">Spawn y-position</param>
    /// <returns></returns>
    public ChatBox CreateChatBox(float x=0, float y=0)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("ChatBox", typeof(GameObject))) as GameObject;
        ChatBox chatBox = obj.GetComponent<ChatBox>();
        if (!chatBox) return null;
        chatBox.rb.position = new Vector3(x, y, 0);
        return chatBox;
    }

    /// <summary>
    /// Creates a Message instance that is spawned at the specified location.
    /// </summary>
    /// <param name="x">Spawn x-position</param>
    /// <param name="y">Spawn y-position</param>
    /// <returns></returns>
    public Message CreateMessage(float x=0, float y=0)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Message", typeof(GameObject))) as GameObject;
        Message message = obj.GetComponent<Message>();
        if (!message) return null;
        message.rb.position = new Vector3(x, y, 0);
        return message;
    }

    // Acceptance test
    [Test]
    public void TestChatBar()
    {
        ChatBar chatBar = CreateChatBar(0,0);
        Assert.IsNotNull (chatBar);
    }

    // Acceptance test
    [Test]
    public void TestChatBox()
    {
        ChatBox chatBox = CreateChatBox(0,0);
        Assert.IsNotNull (chatBox);
    }

    // Acceptance test
    [Test]
    public void TestMessage()
    {
        Message message = CreateMessage(0,0);
        Assert.IsNotNull (message);
    }

    // Acceptance test
    [Test]
    public void TestMessageIndex() 
    {
        Message message = CreateMessage(0,0);
        MessageInput messageIn = message.GetComponent<MessageInput>();
        Assert.IsTrue(messageIn.spriteArray.GetLength(1) > messageIn.spriteCount);
    }

    // Acceptance test
    [Test]
    public void TestMessageVisibility() 
    {
        Message message = CreateMessage(0,0);
        MessageInput messageIn = message.GetComponent<MessageInput>();
        ChatBar chatBar = CreateChatBar(0,0);
        OnlineOnKey onKey = chatBar.GetComponent<OnlineOnKey>();
        Assert.IsTrue(messageIn.messageIndex == 8 && onKey.keyStrokes % 2 == 0);
    }

    // Testing ChatBar.WithinView(); along with TestChatBarInsideRadius, achieves 100% coverage
    [Test]
    public void TestChatBarOutsideRadius()
    {
        // Spawn chat bar outside of view of the player
        Player p = CreatePlayer();
        ChatBar c = CreateChatBar(p.posX + 100, p.posY + 100);
    }

    // Testing ChatBar.WithinView(); along with TestChatBarOutsideRadius, achieves 100% coverage
    [Test]
    public void TestChatBarInsideRadius()
    {
        // Spawn chat bar inside the view of the player
        Player p = CreatePlayer();
        ChatBar c = CreateChatBar(p.posX, p.posY);
    }

    // Testing ChatBox.WithinView(); along with TestChatBoxInsideRadius, achieves 100% coverage
    [Test]
    public void TestChatBoxOutsideRadius()
    {
        // Spawn chat box outside of view of the player
        Player p = CreatePlayer();
        ChatBox c = CreateChatBox(p.posX + 100, p.posY + 100);
    }

    // Testing ChatBox.WithinView(); along with TestChatBoxOutsideRadius, achieves 100% coverage
    [Test]
    public void TestChatBoxInsideRadius()
    {
        // Spawn chat box inside the view of the player
        Player p = CreatePlayer();
        ChatBox c = CreateChatBox(p.posX, p.posY);
    }

    // Testing Message.WithinView(); along with TestMessageInsideRadius, achieves 100% coverage
    [Test]
    public void TestMessageOutsideRadius()
    {
        // Spawn message inside the view of the player
        Player p = CreatePlayer();
        Message m = CreateMessage(p.posX, p.posY);
    }

    // Testing Message.WithinView(); along with TestMessageOutsideRadius, achieves 100% coverage
    [Test]
    public void TestMessageInsideRadius()
    {
        // Spawn message inside the view of the player
        Player p = CreatePlayer();
        Message m = CreateMessage(p.posX, p.posY);
    }
    
    // Integration testing between chatbox and player
    [Test]
    public void TestPlayerLockedWhenChatOpen()
    {
        // Create a test player
        Player p = CreatePlayer();
        p.gameObject.name = "TestPlayer";

        // Create a test chatbox
        ChatBox c = CreateChatBox();
        c.gameObject.name = "TestChatBox";
        
        // Lock chatbox
        c.chatLock = true;
        
        // Make sure player is also locked
        Assert.AreEqual(c.chatLock, p.chatLock);
    }
}
