var counterEl = null;
var svgObj = null;

$(document).ready(function()
{
	svgObj = document.getElementById("svgDoc");
});

$(window).on("load", function()
{
	waitForSVG(svgDoc =>
	{

		counterEl = findDeepestTextNode(svgDoc.getElementById("ProgressNumber01"));
		console.log("SVG ready:", counterEl);

		// Your animation logic goes here
		loop();
	});
});


function animateCount(start, end, duration, callback)
{
	const range = end - start;
	const startTime = performance.now();

	function update(now)
	{
		const progress = Math.min((now - startTime) / duration, 1);
		const value = Math.round(start + range * progress);
		counterEl.textContent = value;

		if(progress < 1)
		{
			requestAnimationFrame(update);
		} else if(callback)
		{
			callback();
		}
	}

	requestAnimationFrame(update);
}

function findDeepestTextNode(el)
{
	let node = el;
	while(node && node.firstChild)
	{
		node = node.firstChild;
	}
	return node;
}

function loop()
{
	animateCount(54, 94, 6000, () =>
	{
		animateCount(93, 53, 6000, loop);
	});
}

var errIndex = 0;
function waitForSVG(callback)
{
	try
	{
		var doc = svgObj.contentDocument;
	}
	catch
	{
		errIndex++;
		console.log(`Error: ${errIndex}`);
	}
	if(doc)
	{
		callback(doc);
	} else
	{
		// Try again on the next animation frame
		requestAnimationFrame(() => waitForSVG(callback));
	}
}

