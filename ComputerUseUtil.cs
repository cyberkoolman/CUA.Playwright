// Copyright (c) Microsoft. All rights reserved.

#pragma warning disable OPENAI001, OPENAICUA001

using OpenAI.Responses;
using PlaywrightLib = Microsoft.Playwright;
using System.Reflection;
using System.Text.Json;

namespace CUA.Playwright;

internal static class ComputerUseUtil
{
    /// <summary>
    /// Execute AI-decided computer action with real Playwright and return new screenshot.
    /// The AI analyzes screenshots to decide what to do - we just execute its commands.
    /// </summary>
    internal static async Task<byte[]> ExecuteComputerActionAsync(
            ComputerCallAction action,
            PlaywrightLib.IPage page)
    {
        Console.WriteLine($"🎬 Executing AI action: {action.Kind}");
        
        // Execute the AI's decision in the real browser
        await ExecuteActionOnPage(action, page);
        
        // Wait for page to settle after action
        await Task.Delay(1500);
        
        // Take new screenshot for AI to analyze
        byte[] screenshot = await page.ScreenshotAsync();
        Console.WriteLine($"   📸 New screenshot captured for AI analysis");
        
        return screenshot;
    }

    private static async Task ExecuteActionOnPage(
        ComputerCallAction action,
        PlaywrightLib.IPage page)
    {
        string actionType = action.Kind.ToString();

        // DEBUG: Print all available properties and fields
        Console.WriteLine($"\n🔍 DEBUG - Action object details:");
        Console.WriteLine($"   Type: {action.GetType().FullName}");
        Console.WriteLine($"   Properties: {string.Join(", ", action.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => $"{p.Name}:{p.PropertyType.Name}"))}");
        Console.WriteLine($"   Fields: {string.Join(", ", action.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select(f => $"{f.Name}:{f.FieldType.Name}"))}");

        try
        {
            // TYPE - AI saw a text field and wants to type in it
            if (actionType.Equals("type", StringComparison.OrdinalIgnoreCase) && action.TypeText is not null)
            {
                Console.WriteLine($"   💬 AI wants to type: '{action.TypeText}'");
                
                // Try to find focused element or first visible input
                var focused = await page.EvaluateAsync<bool>("document.activeElement && (document.activeElement.tagName === 'INPUT' || document.activeElement.tagName === 'TEXTAREA')");
                
                if (focused)
                {
                    // Type in currently focused element
                    await page.Keyboard.TypeAsync(action.TypeText);
                }
                else
                {
                    // Find first visible input/textarea
                    var inputs = await page.QuerySelectorAllAsync("input[type='text'], input[type='search'], textarea");
                    foreach (var input in inputs)
                    {
                        if (await input.IsVisibleAsync())
                        {
                            await input.ClickAsync(); // Focus it
                            await page.Keyboard.TypeAsync(action.TypeText);
                            break;
                        }
                    }
                }
            }

            // KEY - AI wants to press a key (Enter, Tab, etc.)
            // DISABLED: Enter key doesn't work reliably - AI should click search button instead
            else if (actionType.Contains("key", StringComparison.OrdinalIgnoreCase) && action.KeyPressKeyCodes is not null)
            {
                Console.WriteLine($"   ⚠️  KEY PRESS DISABLED - AI should click the search button instead");
                Console.WriteLine($"   ⚠️  Attempted keys: {string.Join(", ", action.KeyPressKeyCodes)}");
                // Skip key press - do nothing
            }

            // CLICK - AI saw something to click at specific coordinates
            else if (actionType.Contains("click", StringComparison.OrdinalIgnoreCase))
            {
                // Try to extract coordinates using reflection since preview API doesn't expose them publicly
                int x = 0, y = 0;
                bool hasCoords = false;

                try
                {
                    // Use reflection to access private/internal coordinate properties
                    var actionTypeObj = action.GetType();
                    
                    // Try common property names for coordinates
                    var xProp = actionTypeObj.GetProperty("X", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var yProp = actionTypeObj.GetProperty("Y", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    if (xProp != null && yProp != null)
                    {
                        var xValue = xProp.GetValue(action);
                        var yValue = yProp.GetValue(action);
                        
                        if (xValue != null && yValue != null)
                        {
                            x = Convert.ToInt32(xValue);
                            y = Convert.ToInt32(yValue);
                            hasCoords = true;
                        }
                    }
                    
                    // If that didn't work, try alternative property names
                    if (!hasCoords)
                    {
                        xProp = actionTypeObj.GetProperty("Coordinate_X", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        yProp = actionTypeObj.GetProperty("Coordinate_Y", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        
                        if (xProp != null && yProp != null)
                        {
                            var xValue = xProp.GetValue(action);
                            var yValue = yProp.GetValue(action);
                            
                            if (xValue != null && yValue != null)
                            {
                                x = Convert.ToInt32(xValue);
                                y = Convert.ToInt32(yValue);
                                hasCoords = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Could not extract coordinates: {ex.Message}");
                }
                
                if (hasCoords)
                {
                    Console.WriteLine($"   🖱️  AI wants to click at ({x}, {y})");
                    await page.Mouse.ClickAsync(x, y);
                }
                else
                {
                    Console.WriteLine($"   🖱️  Click action detected but coordinates not accessible");
                    Console.WriteLine($"   📋 Available properties: {string.Join(", ", action.GetType().GetProperties().Select(p => p.Name))}");
                }
                
                await Task.Delay(1000); // Wait for click to process
            }

            // SCROLL - AI wants to scroll
            else if (actionType.Contains("scroll", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"   📜 AI wants to scroll");
                await page.Mouse.WheelAsync(0, 300); // Default scroll down
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ⚠️  Error executing action: {ex.Message}");
        }
    }

    private static string MapKeyCode(string keyCode)
    {
        // Map AI key codes to Playwright key names
        return keyCode switch
        {
            "Return" => "Enter",
            "Enter" => "Enter",
            _ => keyCode
        };
    }

}
