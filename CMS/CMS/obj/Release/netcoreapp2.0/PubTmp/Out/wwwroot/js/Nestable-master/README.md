Nestable++
========

Drag and drop menu editor jQuery plugin, forked from [dbushell/nestable](https://github.com/dbushell/Nestable).

Additions:
- Add/Delete/Edit menu items
- Slug attribute

![Nestable++ plugin](https://raw.githubusercontent.com/ShinDarth/Nestable/master/screenshot.png "Nestable++ plugin")

## Nestable++ notes

Suppose that all the initial items are already **stored** in your database (or somewhere else):

- On deletion, all stored elements will have **data-deleted** property set to 1
- On deletion, all non-stored elements will be just removed from DOM and from JSON (like they never existed)
- On addition, all new elements will have **data-new** property set to 1 and a value like *new-** as **data-id**

## Usage

Write your nested HTML lists like so:

    <div class="dd">
        <ol class="dd-list">

            <!--- Item1 --->
            <li class="dd-item" data-id="1" data-name="Item 1" data-slug="item-slug-1" data-new="0" data-deleted="0">
              <div class="dd-handle">Item 1</div>
              <span class="button-delete btn btn-default btn-xs pull-right"
                    data-owner-id="1">
                <i class="fa fa-times-circle-o" aria-hidden="true"></i>
              </span>
              <span class="button-edit btn btn-default btn-xs pull-right"
                    data-owner-id="1">
                <i class="fa fa-pencil" aria-hidden="true"></i>
              </span>
            </li>

            <!--- Item2 --->
            <li class="dd-item" data-id="2" data-name="Item 2" data-slug="item-slug-2" data-new="0" data-deleted="0">
              <div class="dd-handle">Item 2</div>
              <span class="button-delete btn btn-default btn-xs pull-right"
                    data-owner-id="2">
                <i class="fa fa-times-circle-o" aria-hidden="true"></i>
              </span>
              <span class="button-edit btn btn-default btn-xs pull-right"
                    data-owner-id="2">
                <i class="fa fa-pencil" aria-hidden="true"></i>
              </span>
            </li>

            <!--- Item3 --->
            <li class="dd-item" data-id="3" data-name="Item 3" data-slug="item-slug-3" data-new="0" data-deleted="0">
              <div class="dd-handle">Item 3</div>
              <span class="button-delete btn btn-default btn-xs pull-right"
                    data-owner-id="3">
                <i class="fa fa-times-circle-o" aria-hidden="true"></i>
              </span>
              <span class="button-edit btn btn-default btn-xs pull-right"
                    data-owner-id="3">
                <i class="fa fa-pencil" aria-hidden="true"></i>
              </span>
              <!--- Item3 children --->
              <ol class="dd-list">
                <!--- Item4 --->
                <li class="dd-item" data-id="4" data-name="Item 4" data-slug="item-slug-4" data-new="0" data-deleted="0">
                  <div class="dd-handle">Item 4</div>
                  <span class="button-delete btn btn-default btn-xs pull-right"
                        data-owner-id="4">
                    <i class="fa fa-times-circle-o" aria-hidden="true"></i>
                  </span>
                  <span class="button-edit btn btn-default btn-xs pull-right"
                        data-owner-id="4">
                    <i class="fa fa-pencil" aria-hidden="true"></i>
                  </span>
                </li>

                <!--- Item5 --->
                <li class="dd-item" data-id="5" data-name="Item 5" data-slug="item-slug-5" data-new="0" data-deleted="0">
                  <div class="dd-handle">Item 5</div>
                  <span class="button-delete btn btn-default btn-xs pull-right"
                        data-owner-id="5">
                    <i class="fa fa-times-circle-o" aria-hidden="true"></i>
                  </span>
                  <span class="button-edit btn btn-default btn-xs pull-right"
                        data-owner-id="5">
                    <i class="fa fa-pencil" aria-hidden="true"></i>
                  </span>
                </li>
              </ol>
            </li>

        </ol>
    </div>

Then activate with jQuery like so:

    $('.dd').nestable({ /* config options */ });

### Events

The `change` event is fired when items are reordered.

    $('.dd').on('change', function() {
        /* on change event */
    });

### Methods

You can get a serialised object with all `data-*` attributes for each item.

    $('.dd').nestable('serialize');

The serialised JSON for the example above would be:

    [{"id":1},{"id":2},{"id":3,"children":[{"id":4},{"id":5}]}]

### Configuration

You can change the follow options:

* `maxDepth` number of levels an item can be nested (default `5`)
* `group` group ID to allow dragging between lists (default `0`)

These advanced config options are also available:

* `listNodeName` The HTML element to create for lists (default `'ol'`)
* `itemNodeName` The HTML element to create for list items (default `'li'`)
* `rootClass` The class of the root element `.nestable()` was used on (default `'dd'`)
* `listClass` The class of all list elements (default `'dd-list'`)
* `itemClass` The class of all list item elements (default `'dd-item'`)
* `dragClass` The class applied to the list element that is being dragged (default `'dd-dragel'`)
* `handleClass` The class of the content element inside each list item (default `'dd-handle'`)
* `collapsedClass` The class applied to lists that have been collapsed (default `'dd-collapsed'`)
* `placeClass` The class of the placeholder element (default `'dd-placeholder'`)
* `emptyClass` The class used for empty list placeholder elements (default `'dd-empty'`)
* `expandBtnHTML` The HTML text used to generate a list item expand button (default `'<button data-action="expand">Expand></button>'`)
* `collapseBtnHTML` The HTML text used to generate a list item collapse button (default `'<button data-action="collapse">Collapse</button>'`)

Inspect the [Nestable Demo](http://dbushell.github.com/Nestable/) for guidance of the original Nestable component (created by David Bushell).
