const names = ["WaterX", "PotatoTomato", "AcidBunny", "Loco", "Genius", "Cuddly-Wuddly", "Squab", "Buju", "Lollichop", "PyroHome", "RumpleThump", "Fusionbreak",
    "AstraGirl", "JulesCrown", "HoltHamlet", "MsMittens", "ImpPlant", "SandySun", "BloodSoul", "CastleClimb", "Crystalrage", "Tommygun", "Attacktheattacker", "Ouster",
    "AStroBoy", "Deadlight", "Crazywar", "Pilar", "Salvostrike", "MystiqueMamba", "NumbLeg", "Octopi", "Underfire41", "FryerTuck91", "OculusVision31", "Commandame1",
    "TrueFateEdge", "EdgeLord69", "Minkx0", "RustySilver", "Praltiller61", "HeroIceWallow", "Kerplunk9062"];

var count = 1;
function load(c) {

    var div = document.getElementById('mena1');
    
    let a = Math.floor(Math.random() * 43);
    let name = names[a];
    var progress=document.getElementById('progress');
    $(progress).animate({width:'100%'},c-500);
    var el = document.getElementById('buy');
    el.innerText = name;
    let b = Math.floor(Math.random() * 120) + 25;
    var el2 = document.getElementById('deposit');
    var depwithr=Math.floor(Math.random()*10);
    if(depwithr>=4)
    {

        el2.innerText = "deposited: " + b + " $" ;
        
    }
    else{
        
        el2.innerText = "withdrew: " + b + " $";
    }
    $(div).addClass('animate__backInRight');
    $(div).removeClass('animate__backOutLeft');
    count++;
}
function unload() {
    var progress=document.getElementById('progress');
    $(progress).css('width','0%');
    var div = document.getElementById('mena1');
    $(div).removeClass('animate__backInRight');
    $(div).addClass('animate__backOutLeft');
    count--;
}
$(document).ready(function () {
    var c= Math.floor(Math.random()*12000)+5000;
    load(c);
    setInterval(function () {
        
        if (count == 1) {
           load(c);
        }
        else {
            unload(c);
        }
    }, c);
});